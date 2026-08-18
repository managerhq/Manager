using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using SQLitePCL;

namespace ManagerServer.Orm
{
    public sealed class SQLiteConnection : IDisposable
    {
        private static bool _initialized;

        public sqlite3 Handle { get; private set; }
        public bool Trace { get; set; }
        public Action<string> Tracer { get; set; }

        private TimeSpan _busyTimeout;
        public TimeSpan BusyTimeout
        {
            get => _busyTimeout;
            set
            {
                _busyTimeout = value;
                raw.sqlite3_busy_timeout(Handle, (int)value.TotalMilliseconds);
            }
        }

        public SQLiteConnection(string databasePath)
        {
            if (!_initialized)
            {
                Batteries_V2.Init();
                _initialized = true;
            }

            var rc = raw.sqlite3_open_v2(databasePath, out var handle, raw.SQLITE_OPEN_READWRITE | raw.SQLITE_OPEN_CREATE, null);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((SQLite3.Result)rc, $"Could not open database: {databasePath}");
            Handle = handle;
        }

        public void Dispose()
        {
            if (Handle != null)
            {
                raw.sqlite3_close(Handle);
                Handle = null;
            }
        }

        // --- Transactions ---

        public SQLiteTransaction BeginTransaction()
        {
            return new SQLiteTransaction(this);
        }

        // --- Maintenance ---

        public void Vacuum() => ExecuteNonQuery("VACUUM");

        public void Pragma(string pragma) => ExecuteNonQuery("PRAGMA " + pragma);

        // --- LINQ entry point ---

        public TableQuery<T> Table<T>() where T : new()
        {
            return new TableQuery<T>(this);
        }

        // --- Read operations ---

        public T Find<T>(object primaryKey) where T : new()
        {
            var mapping = TableMapping.Get<T>();
            if (mapping.PrimaryKey == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} has no primary key");

            return Query<T>($"SELECT * FROM \"{mapping.TableName}\" WHERE \"{mapping.PrimaryKey.Name}\" = ?", primaryKey)
                .FirstOrDefault();
        }

        public T Get<T>(object primaryKey) where T : new()
        {
            var result = Find<T>(primaryKey);
            if (result == null)
                throw new InvalidOperationException($"{typeof(T).Name} with primary key {primaryKey} not found");
            return result;
        }

        // --- Raw SQL ---

        public List<T> Query<T>(string sql, params object[] args) where T : new()
        {
            var mapping = TableMapping.Get<T>();
            var results = new List<T>();

            if (Trace && Tracer != null) Tracer(sql);

            var rc = raw.sqlite3_prepare_v2(Handle, sql, out var stmt);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((SQLite3.Result)rc, raw.sqlite3_errmsg(Handle).utf8_to_string());

            try
            {
                for (int i = 0; i < args.Length; i++)
                    BindParameter(stmt, i + 1, args[i]);

                var columnCount = raw.sqlite3_column_count(stmt);
                var columnMap = new Dictionary<string, int>(columnCount, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < columnCount; i++)
                {
                    var name = raw.sqlite3_column_name(stmt, i).utf8_to_string();
                    columnMap[name] = i;
                }

                while (raw.sqlite3_step(stmt) == raw.SQLITE_ROW)
                {
                    var obj = new T();
                    foreach (var col in mapping.Columns)
                    {
                        if (columnMap.TryGetValue(col.Name, out var idx))
                        {
                            var value = ReadColumn(stmt, idx, col.ClrType);
                            if (value != null)
                                col.SetValue(obj, value);
                        }
                    }
                    results.Add(obj);
                }
            }
            finally
            {
                raw.sqlite3_finalize(stmt);
            }

            return results;
        }

        public List<T> QueryScalars<T>(string sql, params object[] args)
        {
            var results = new List<T>();

            if (Trace && Tracer != null) Tracer(sql);

            var rc = raw.sqlite3_prepare_v2(Handle, sql, out var stmt);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((SQLite3.Result)rc, raw.sqlite3_errmsg(Handle).utf8_to_string());

            try
            {
                for (int i = 0; i < args.Length; i++)
                    BindParameter(stmt, i + 1, args[i]);

                while (raw.sqlite3_step(stmt) == raw.SQLITE_ROW)
                {
                    var value = ReadColumn(stmt, 0, typeof(T));
                    results.Add(value != null ? (T)value : default);
                }
            }
            finally
            {
                raw.sqlite3_finalize(stmt);
            }

            return results;
        }

        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            if (Trace && Tracer != null) Tracer(sql);

            var rc = raw.sqlite3_prepare_v2(Handle, sql, out var stmt);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((SQLite3.Result)rc, raw.sqlite3_errmsg(Handle).utf8_to_string());

            try
            {
                for (int i = 0; i < args.Length; i++)
                    BindParameter(stmt, i + 1, args[i]);

                if (raw.sqlite3_step(stmt) == raw.SQLITE_ROW)
                {
                    var value = ReadColumn(stmt, 0, typeof(T));
                    return value != null ? (T)value : default;
                }

                return default;
            }
            finally
            {
                raw.sqlite3_finalize(stmt);
            }
        }

        internal int ExecuteNonQuery(string sql, params object[] args)
        {
            if (Trace && Tracer != null) Tracer(sql);

            var rc = raw.sqlite3_prepare_v2(Handle, sql, out var stmt);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((SQLite3.Result)rc, raw.sqlite3_errmsg(Handle).utf8_to_string());

            try
            {
                for (int i = 0; i < args.Length; i++)
                    BindParameter(stmt, i + 1, args[i]);

                var result = raw.sqlite3_step(stmt);
                if (result != raw.SQLITE_DONE && result != raw.SQLITE_ROW)
                    throw new SQLiteException((SQLite3.Result)result, raw.sqlite3_errmsg(Handle).utf8_to_string());

                return raw.sqlite3_changes(Handle);
            }
            finally
            {
                raw.sqlite3_finalize(stmt);
            }
        }

        // --- Backup ---

        public void Backup(string destinationPath)
        {
            var rc = raw.sqlite3_open_v2(destinationPath, out var destHandle, raw.SQLITE_OPEN_READWRITE | raw.SQLITE_OPEN_CREATE, null);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((SQLite3.Result)rc, $"Could not open backup destination: {destinationPath}");

            try
            {
                var backup = raw.sqlite3_backup_init(destHandle, "main", Handle, "main");
                if (backup == null)
                    throw new SQLiteException(SQLite3.Result.Error, raw.sqlite3_errmsg(destHandle).utf8_to_string());

                raw.sqlite3_backup_step(backup, -1);
                raw.sqlite3_backup_finish(backup);
            }
            finally
            {
                raw.sqlite3_close(destHandle);
            }
        }

        // --- Type mapping ---

        internal static void BindParameter(sqlite3_stmt stmt, int index, object value)
        {
            if (value == null)
            {
                raw.sqlite3_bind_null(stmt, index);
            }
            else if (value is string s)
            {
                raw.sqlite3_bind_text(stmt, index, s);
            }
            else if (value is int i)
            {
                raw.sqlite3_bind_int(stmt, index, i);
            }
            else if (value is long l)
            {
                raw.sqlite3_bind_int64(stmt, index, l);
            }
            else if (value is bool b)
            {
                raw.sqlite3_bind_int(stmt, index, b ? 1 : 0);
            }
            else if (value is Guid g)
            {
                raw.sqlite3_bind_text(stmt, index, g.ToString());
            }
            else if (value is byte[] bytes)
            {
                raw.sqlite3_bind_blob(stmt, index, bytes);
            }
            else if (value is double d)
            {
                raw.sqlite3_bind_double(stmt, index, d);
            }
            else if (value is float f)
            {
                raw.sqlite3_bind_double(stmt, index, f);
            }
            else if (value is DateTime dt)
            {
                raw.sqlite3_bind_text(stmt, index, dt.ToString("o"));
            }
            else
            {
                raw.sqlite3_bind_text(stmt, index, value.ToString());
            }
        }

        internal static object ReadColumn(sqlite3_stmt stmt, int index, Type targetType)
        {
            var colType = raw.sqlite3_column_type(stmt, index);
            if (colType == raw.SQLITE_NULL) return null;

            if (targetType == typeof(string))
            {
                return raw.sqlite3_column_text(stmt, index).utf8_to_string();
            }
            else if (targetType == typeof(int))
            {
                return raw.sqlite3_column_int(stmt, index);
            }
            else if (targetType == typeof(long))
            {
                return raw.sqlite3_column_int64(stmt, index);
            }
            else if (targetType == typeof(bool))
            {
                return raw.sqlite3_column_int(stmt, index) != 0;
            }
            else if (targetType == typeof(Guid))
            {
                var text = raw.sqlite3_column_text(stmt, index).utf8_to_string();
                return Guid.Parse(text);
            }
            else if (targetType == typeof(byte[]))
            {
                return raw.sqlite3_column_blob(stmt, index).ToArray();
            }
            else if (targetType == typeof(double))
            {
                return raw.sqlite3_column_double(stmt, index);
            }
            else if (targetType == typeof(float))
            {
                return (float)raw.sqlite3_column_double(stmt, index);
            }
            else if (targetType == typeof(DateTime))
            {
                var text = raw.sqlite3_column_text(stmt, index).utf8_to_string();
                return DateTime.Parse(text);
            }
            else
            {
                return raw.sqlite3_column_text(stmt, index).utf8_to_string();
            }
        }

        internal static string GetSqliteType(Type clrType)
        {
            if (clrType == typeof(string) || clrType == typeof(Guid) || clrType == typeof(DateTime))
                return "TEXT";
            if (clrType == typeof(int) || clrType == typeof(long) || clrType == typeof(bool))
                return "INTEGER";
            if (clrType == typeof(byte[]))
                return "BLOB";
            if (clrType == typeof(double) || clrType == typeof(float))
                return "REAL";
            return "TEXT";
        }
    }
}

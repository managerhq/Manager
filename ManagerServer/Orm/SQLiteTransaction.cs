using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Orm
{
    public sealed class SQLiteTransaction : IDisposable
    {
        private readonly SQLiteConnection _connection;
        private bool _committed;

        internal SQLiteTransaction(SQLiteConnection connection)
        {
            _connection = connection;
            connection.ExecuteNonQuery("BEGIN TRANSACTION");
        }

        public int Execute(string sql, params object[] args)
        {
            var result = _connection.ExecuteNonQuery(sql, args);
            return result;
        }

        public void CreateTable<T>()
        {
            var mapping = TableMapping.Get<T>();
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE IF NOT EXISTS \"").Append(mapping.TableName).Append("\" (");

            for (int i = 0; i < mapping.Columns.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                var col = mapping.Columns[i];
                sb.Append('"').Append(col.Name).Append("\" ").Append(SQLiteConnection.GetSqliteType(col.ClrType));
                if (col.IsPrimaryKey) sb.Append(" PRIMARY KEY");
            }

            sb.Append(')');
            if (mapping.WithoutRowId) sb.Append(" WITHOUT ROWID");

            Execute(sb.ToString());

            var existingColumns = new HashSet<string>(
                _connection.QueryScalars<string>($"SELECT name FROM pragma_table_info('{mapping.TableName}')"),
                StringComparer.OrdinalIgnoreCase);

            foreach (var col in mapping.Columns)
            {
                if (col.IsPrimaryKey) continue;
                if (existingColumns.Contains(col.Name)) continue;
                Execute($"ALTER TABLE \"{mapping.TableName}\" ADD COLUMN \"{col.Name}\" {SQLiteConnection.GetSqliteType(col.ClrType)}");
            }

            foreach (var col in mapping.Columns.Where(c => c.IsIndexed))
            {
                Execute($"CREATE INDEX IF NOT EXISTS \"ix_{mapping.TableName}_{col.Name}\" ON \"{mapping.TableName}\" (\"{col.Name}\")");
            }
        }

        public void DropTable<T>()
        {
            var mapping = TableMapping.Get<T>();
            Execute($"DROP TABLE IF EXISTS \"{mapping.TableName}\"");
        }

        public int Insert(object obj)
        {
            var mapping = TableMapping.Get(obj.GetType());
            var columns = mapping.Columns;

            var sb = new StringBuilder();
            sb.Append("INSERT INTO \"").Append(mapping.TableName).Append("\" (");
            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append('"').Append(columns[i].Name).Append('"');
            }
            sb.Append(") VALUES (");
            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append('?');
            }
            sb.Append(')');

            var args = columns.Select(c => c.GetValue(obj)).ToArray();
            return Execute(sb.ToString(), args);
        }

        public int InsertOrReplace(object obj)
        {
            var mapping = TableMapping.Get(obj.GetType());
            var columns = mapping.Columns;

            var sb = new StringBuilder();
            sb.Append("INSERT OR REPLACE INTO \"").Append(mapping.TableName).Append("\" (");
            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append('"').Append(columns[i].Name).Append('"');
            }
            sb.Append(") VALUES (");
            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append('?');
            }
            sb.Append(')');

            var args = columns.Select(c => c.GetValue(obj)).ToArray();
            return Execute(sb.ToString(), args);
        }

        public int Delete<T>(object primaryKey)
        {
            var mapping = TableMapping.Get<T>();
            if (mapping.PrimaryKey == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} has no primary key");

            return Execute($"DELETE FROM \"{mapping.TableName}\" WHERE \"{mapping.PrimaryKey.Name}\" = ?", primaryKey);
        }

        public void Commit()
        {
            _connection.ExecuteNonQuery("COMMIT");
            _committed = true;
        }

        public void Dispose()
        {
            if (!_committed)
                _connection.ExecuteNonQuery("ROLLBACK");
        }        
    }
}

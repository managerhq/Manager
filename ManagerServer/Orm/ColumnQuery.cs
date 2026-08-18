using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SQLitePCL;

namespace ManagerServer.Orm
{
    public sealed class ColumnQuery<TResult> : IEnumerable<TResult>
    {
        private readonly SQLiteConnection _connection;
        private readonly string _tableName;
        private readonly string _columnName;
        private readonly string _whereSql;
        private readonly IReadOnlyList<object> _whereParameters;
        private readonly bool _distinct;

        internal ColumnQuery(SQLiteConnection connection, string tableName, string columnName, string whereSql, IReadOnlyList<object> whereParameters, bool distinct)
        {
            _connection = connection;
            _tableName = tableName;
            _columnName = columnName;
            _whereSql = whereSql;
            _whereParameters = whereParameters;
            _distinct = distinct;
        }

        public ColumnQuery<TResult> Distinct()
        {
            return new ColumnQuery<TResult>(_connection, _tableName, _columnName, _whereSql, _whereParameters, true);
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            var sb = new StringBuilder();
            sb.Append(_distinct ? "SELECT DISTINCT" : "SELECT");
            sb.Append(" \"").Append(_columnName).Append("\" FROM \"").Append(_tableName).Append('"');
            if (!string.IsNullOrEmpty(_whereSql))
                sb.Append(" WHERE ").Append(_whereSql);

            var sql = sb.ToString();

            if (_connection.Trace && _connection.Tracer != null)
                _connection.Tracer(sql);

            var rc = raw.sqlite3_prepare_v2(_connection.Handle, sql, out var stmt);
            if (rc != raw.SQLITE_OK)
            {
                var msg = raw.sqlite3_errmsg(_connection.Handle).utf8_to_string();
                if (msg != null && msg.Contains("no such table"))
                    yield break;
                throw new SQLiteException((SQLite3.Result)rc, msg);
            }

            try
            {
                for (int i = 0; i < _whereParameters.Count; i++)
                    SQLiteConnection.BindParameter(stmt, i + 1, _whereParameters[i]);

                while (raw.sqlite3_step(stmt) == raw.SQLITE_ROW)
                {
                    var value = SQLiteConnection.ReadColumn(stmt, 0, typeof(TResult));
                    yield return value != null ? (TResult)value : default;
                }
            }
            finally
            {
                raw.sqlite3_finalize(stmt);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

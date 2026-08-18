using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SQLitePCL;

namespace ManagerServer.Orm
{
    public sealed class TableQuery<T> : IEnumerable<T> where T : new()
    {
        private readonly SQLiteConnection _connection;
        private readonly TableMapping _mapping;
        private readonly List<Expression<Func<T, bool>>> _wherePredicates;
        private readonly string _orderByColumn;
        private readonly bool _orderByDescending;
        private readonly int? _skip;
        private readonly int? _take;

        internal TableQuery(SQLiteConnection connection)
        {
            _connection = connection;
            _mapping = TableMapping.Get<T>();
            _wherePredicates = new List<Expression<Func<T, bool>>>();
        }

        private TableQuery(SQLiteConnection connection, TableMapping mapping, List<Expression<Func<T, bool>>> predicates, string orderByColumn, bool orderByDescending, int? skip, int? take)
        {
            _connection = connection;
            _mapping = mapping;
            _wherePredicates = predicates;
            _orderByColumn = orderByColumn;
            _orderByDescending = orderByDescending;
            _skip = skip;
            _take = take;
        }

        public TableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            var predicates = new List<Expression<Func<T, bool>>>(_wherePredicates) { predicate };
            return new TableQuery<T>(_connection, _mapping, predicates, _orderByColumn, _orderByDescending, _skip, _take);
        }

        public TableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var column = GetMemberName(keySelector.Body);
            return new TableQuery<T>(_connection, _mapping, _wherePredicates, column, false, _skip, _take);
        }

        public TableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var column = GetMemberName(keySelector.Body);
            return new TableQuery<T>(_connection, _mapping, _wherePredicates, column, true, _skip, _take);
        }

        public TableQuery<T> Skip(int n)
        {
            return new TableQuery<T>(_connection, _mapping, _wherePredicates, _orderByColumn, _orderByDescending, n, _take);
        }

        public TableQuery<T> Take(int n)
        {
            return new TableQuery<T>(_connection, _mapping, _wherePredicates, _orderByColumn, _orderByDescending, _skip, n);
        }

        public int Count()
        {
            try
            {
                var (sql, parameters) = BuildSql(countOnly: true);
                return _connection.ExecuteScalar<int>(sql, parameters.ToArray());
            }
            catch (SQLiteException ex) when (IsNoSuchTable(ex))
            {
                return 0;
            }
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate).Count();
        }

        public T FirstOrDefault()
        {
            return Take(1).GetEnumerator().MoveNext() ? Take(1).First() : default;
        }

        public ColumnQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            var columnName = GetMemberName(selector.Body);

            // Build WHERE clause from current predicates
            var whereSql = new StringBuilder();
            var whereParameters = new List<object>();
            for (int i = 0; i < _wherePredicates.Count; i++)
            {
                if (i > 0) whereSql.Append(" AND ");
                var predicate = _wherePredicates[i];
                var visitor = new WhereExpressionVisitor(predicate.Parameters[0]);
                visitor.Translate(predicate.Body);
                whereSql.Append('(').Append(visitor.Sql).Append(')');
                whereParameters.AddRange(visitor.Parameters);
            }

            return new ColumnQuery<TResult>(_connection, _mapping.TableName, columnName, whereSql.ToString(), whereParameters, false);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var (sql, parameters) = BuildSql(countOnly: false);

            if (_connection.Trace && _connection.Tracer != null)
                _connection.Tracer(sql);

            var rc = raw.sqlite3_prepare_v2(_connection.Handle, sql, out var stmtOut);
            if (rc != raw.SQLITE_OK)
            {
                var msg = raw.sqlite3_errmsg(_connection.Handle).utf8_to_string();
                if (msg != null && msg.Contains("no such table"))
                    yield break;
                throw new SQLiteException((SQLite3.Result)rc, msg);
            }

            try
            {
                for (int i = 0; i < parameters.Count; i++)
                    SQLiteConnection.BindParameter(stmtOut, i + 1, parameters[i]);

                // Build column name -> index mapping from the result set
                var columnCount = raw.sqlite3_column_count(stmtOut);
                var columnMap = new Dictionary<string, int>(columnCount, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < columnCount; i++)
                {
                    var name = raw.sqlite3_column_name(stmtOut, i).utf8_to_string();
                    columnMap[name] = i;
                }

                while (raw.sqlite3_step(stmtOut) == raw.SQLITE_ROW)
                {
                    var obj = new T();
                    foreach (var col in _mapping.Columns)
                    {
                        if (columnMap.TryGetValue(col.Name, out var idx))
                        {
                            var value = SQLiteConnection.ReadColumn(stmtOut, idx, col.ClrType);
                            if (value != null)
                                col.SetValue(obj, value);
                        }
                    }
                    yield return obj;
                }
            }
            finally
            {
                raw.sqlite3_finalize(stmtOut);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private (string sql, List<object> parameters) BuildSql(bool countOnly)
        {
            var sb = new StringBuilder();
            var parameters = new List<object>();

            if (countOnly)
                sb.Append("SELECT COUNT(*) FROM \"").Append(_mapping.TableName).Append('"');
            else
                sb.Append("SELECT * FROM \"").Append(_mapping.TableName).Append('"');

            if (_wherePredicates.Count > 0)
            {
                sb.Append(" WHERE ");
                for (int i = 0; i < _wherePredicates.Count; i++)
                {
                    if (i > 0) sb.Append(" AND ");
                    var predicate = _wherePredicates[i];
                    var visitor = new WhereExpressionVisitor(predicate.Parameters[0]);
                    visitor.Translate(predicate.Body);
                    sb.Append('(').Append(visitor.Sql).Append(')');
                    parameters.AddRange(visitor.Parameters);
                }
            }

            if (!countOnly)
            {
                if (_orderByColumn != null)
                {
                    sb.Append(" ORDER BY \"").Append(_orderByColumn).Append('"');
                    if (_orderByDescending) sb.Append(" DESC");
                }

                if (_take.HasValue)
                {
                    sb.Append(" LIMIT ?");
                    parameters.Add(_take.Value);
                }

                if (_skip.HasValue)
                {
                    if (!_take.HasValue)
                    {
                        sb.Append(" LIMIT -1");
                    }
                    sb.Append(" OFFSET ?");
                    parameters.Add(_skip.Value);
                }
            }

            return (sb.ToString(), parameters);
        }

        private static string GetMemberName(Expression expr)
        {
            if (expr is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                expr = unary.Operand;
            if (expr is MemberExpression member)
                return member.Member.Name;
            throw new NotSupportedException("OrderBy expression must be a simple member access");
        }

        private static bool IsNoSuchTable(SQLiteException ex)
        {
            return ex.Message != null && ex.Message.Contains("no such table");
        }
    }
}

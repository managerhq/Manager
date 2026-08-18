using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ManagerServer.Orm
{
    internal sealed class WhereExpressionVisitor
    {
        private readonly StringBuilder _sql = new();
        private readonly List<object> _parameters = new();
        private readonly ParameterExpression _rowParameter;

        public string Sql => _sql.ToString();
        public IReadOnlyList<object> Parameters => _parameters;

        public WhereExpressionVisitor(ParameterExpression rowParameter)
        {
            _rowParameter = rowParameter;
        }

        public void Translate(Expression expr)
        {
            switch (expr)
            {
                case BinaryExpression binary:
                    TranslateBinary(binary);
                    break;
                case MethodCallExpression call:
                    TranslateMethodCall(call);
                    break;
                case UnaryExpression unary:
                    TranslateUnary(unary);
                    break;
                case MemberExpression member when IsColumnAccess(member):
                    _sql.Append('"').Append(member.Member.Name).Append('"');
                    break;
                case ConstantExpression constant:
                    _sql.Append('?');
                    _parameters.Add(constant.Value);
                    break;
                default:
                    // Try to evaluate as a constant
                    var value = Evaluate(expr);
                    _sql.Append('?');
                    _parameters.Add(value);
                    break;
            }
        }

        private void TranslateBinary(BinaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.AndAlso)
            {
                _sql.Append('(');
                Translate(expr.Left);
                _sql.Append(" AND ");
                Translate(expr.Right);
                _sql.Append(')');
                return;
            }

            if (expr.NodeType == ExpressionType.OrElse)
            {
                _sql.Append('(');
                Translate(expr.Left);
                _sql.Append(" OR ");
                Translate(expr.Right);
                _sql.Append(')');
                return;
            }

            // Comparison operators
            var left = expr.Left;
            var right = expr.Right;

            // Strip Convert nodes (common with nullable/enum comparisons)
            if (left is UnaryExpression ul && ul.NodeType == ExpressionType.Convert) left = ul.Operand;
            if (right is UnaryExpression ur && ur.NodeType == ExpressionType.Convert) right = ur.Operand;

            // Check for null comparison
            bool rightIsNull = IsNull(right);
            bool leftIsNull = IsNull(left);

            if (rightIsNull || leftIsNull)
            {
                var nonNull = rightIsNull ? left : right;
                Translate(nonNull);
                _sql.Append(expr.NodeType == ExpressionType.Equal ? " IS NULL" : " IS NOT NULL");
                return;
            }

            Translate(left);
            _sql.Append(expr.NodeType switch
            {
                ExpressionType.Equal => " = ",
                ExpressionType.NotEqual => " <> ",
                ExpressionType.GreaterThan => " > ",
                ExpressionType.GreaterThanOrEqual => " >= ",
                ExpressionType.LessThan => " < ",
                ExpressionType.LessThanOrEqual => " <= ",
                _ => throw new NotSupportedException($"Binary operator {expr.NodeType} is not supported")
            });
            Translate(right);
        }

        private void TranslateMethodCall(MethodCallExpression expr)
        {
            // Handle Contains for IN clauses
            // Pattern 1: Enumerable.Contains(collection, item) — static method
            // Pattern 2: collection.Contains(item) — instance method
            if (expr.Method.Name == "Contains")
            {
                Expression collectionExpr;
                Expression itemExpr;

                if (expr.Method.IsStatic && expr.Arguments.Count == 2)
                {
                    // Enumerable.Contains(source, value)
                    collectionExpr = expr.Arguments[0];
                    itemExpr = expr.Arguments[1];
                }
                else if (!expr.Method.IsStatic && expr.Arguments.Count == 1)
                {
                    // collection.Contains(value)
                    collectionExpr = expr.Object;
                    itemExpr = expr.Arguments[0];
                }
                else
                {
                    throw new NotSupportedException("Unsupported Contains overload");
                }

                // itemExpr should be a column access
                if (!IsColumnAccess(itemExpr))
                    throw new NotSupportedException("Contains item must be a column access");

                var collection = (IEnumerable)Evaluate(collectionExpr);
                var items = new List<object>();
                foreach (var item in collection) items.Add(item);

                Translate(itemExpr);
                _sql.Append(" IN (");
                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0) _sql.Append(", ");
                    _sql.Append('?');
                    _parameters.Add(items[i]);
                }
                _sql.Append(')');
                return;
            }

            throw new NotSupportedException($"Method {expr.Method.Name} is not supported in WHERE clauses");
        }

        private void TranslateUnary(UnaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.Not)
            {
                _sql.Append("NOT (");
                Translate(expr.Operand);
                _sql.Append(')');
            }
            else if (expr.NodeType == ExpressionType.Convert)
            {
                Translate(expr.Operand);
            }
            else
            {
                throw new NotSupportedException($"Unary operator {expr.NodeType} is not supported");
            }
        }

        private bool IsColumnAccess(Expression expr)
        {
            if (expr is MemberExpression member)
                return member.Expression == _rowParameter;
            return false;
        }

        private bool IsNull(Expression expr)
        {
            if (expr is ConstantExpression c) return c.Value == null;
            return false;
        }

        private static object Evaluate(Expression expr)
        {
            if (expr is ConstantExpression c) return c.Value;
            var lambda = Expression.Lambda(expr);
            return lambda.Compile().DynamicInvoke();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class ExpressionAttribute : Attribute
    {
        private object[] expression;

        public ExpressionAttribute(params object[] expression)
        {
            this.expression = expression;
        }

        public enum Operators
        {
            Zero,
            PlusArray,
            MinusArray,
            AbsoluteValue,
            Negate,
            Round,
            Plus,
            Divide,
            Minus,
            Times,
            TimesPercentage,
            TimesTaxCode,
            IfNullThen,
            RoundToBaseCurrency
        }

        public bool IsDecimal
        {
            get
            {
                return expression.Contains(Operators.Zero);
            }
        }

        public string GetExpression()
        {
            var queue = new Queue<object>(expression);

            var sb = new StringBuilder();
            while (queue.Count > 0)
            {
                var o = queue.Dequeue();
                if (o is Operators)
                {
                    switch (o)
                    {
                        case Operators.Zero:
                            sb.Append("new Decimal(0)");
                            break;
                        case Operators.AbsoluteValue:
                            sb.Append(".abs()");
                            break;
                        case Operators.Negate:
                            sb.Append(".neg()");
                            break;
                        case Operators.Round:
                            sb.Append(".toDecimalPlaces(this.getCurrencyDecimalPlaces())");
                            break;
                        case Operators.RoundToBaseCurrency:
                            sb.Append(".toDecimalPlaces(baseCurrency.decimalPlaces)");
                            break;
                        case Operators.Plus:
                            var plusField = queue.Dequeue() as string;
                            var plusExpression = $"this.get{plusField}(typeof lineItem === typeof undefined ? null : lineItem)";
                            sb.Append($".plus({plusExpression} != null ? {plusExpression} : 0)");
                            break;
                        case Operators.Minus:
                            var minusField = queue.Dequeue() as string;
                            sb.Append($".minus(new Decimal(this.get{minusField}(typeof lineItem === typeof undefined ? null : lineItem)))");
                            break;
                        case Operators.PlusArray:
                            var plusArrayField = queue.Dequeue() as string;
                            sb.Append($".plus(this.get{plusArrayField}Array().reduce((x, y) => x.plus(y), new Decimal(0)))");
                            break;
                        case Operators.MinusArray:
                            var minusArrayField = queue.Dequeue() as string;
                            sb.Append($".plus(this.get{minusArrayField}Array().reduce((x, y) => x.minus(y), new Decimal(0)))");
                            break;
                        case Operators.Times:
                            var timesField = queue.Dequeue() as string;
                            var timesExpression = $"this.get{timesField}(typeof lineItem === typeof undefined ? null : lineItem)";
                            sb.Append($".times({timesExpression} != null ? {timesExpression} : 1)");
                            break;
                        case Operators.Divide:
                            var divideField = queue.Dequeue() as string;
                            var divideExpression = $"this.get{divideField}(typeof lineItem === typeof undefined ? null : lineItem)";
                            sb.Append($".div({divideExpression} != null && {divideExpression} != 0 ? {divideExpression} : 1)");
                            break;
                        case Operators.TimesPercentage:
                            var timesPercentageField = queue.Dequeue() as string;
                            sb.Append($".times(new Decimal(1).minus(new Decimal(this.get{timesPercentageField}(typeof lineItem === typeof undefined ? null : lineItem)).dividedBy(100)))");
                            break;
                        case Operators.TimesTaxCode:
                            var timesTaxCodeField = queue.Dequeue() as string;
                            var expression = sb.ToString();
                            sb.Clear();
                            sb.Append($"(lineItem.{timesTaxCodeField} == null || lineItem.{timesTaxCodeField}.Rates == null) ? new Decimal(0) : lineItem.{timesTaxCodeField}.Rates.map(x => {expression}.times(new Decimal(x).dividedBy(100)).toDecimalPlaces(this.getCurrencyDecimalPlaces())).reduce(function (item1, item2) {{ return item1.plus(item2); }}, new Decimal(0))");
                            break;
                        case Operators.IfNullThen:
                            if (sb.Length > 0) sb.Append(" || ");
                            var s = string.Empty;
                            while (queue.Count > 0 && queue.Peek() is string)
                            {
                                var ifNullThenField = queue.Dequeue() as string;
                                if (string.IsNullOrWhiteSpace(s))
                                {
                                    s = $"typeof lineItem === typeof undefined ? this.{ifNullThenField} : lineItem.{ifNullThenField}";
                                }
                                else
                                {
                                    s = "(" + s + " || {})." + ifNullThenField;
                                }
                            }
                            sb.Append(s);
                            break;
                    }
                }
            }
            return sb.ToString();
        }
    }
}

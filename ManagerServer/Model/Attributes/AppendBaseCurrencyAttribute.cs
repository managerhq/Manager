using System;

namespace ManagerServer.Model.Attributes
{
    public sealed class AppendBaseCurrencyAttribute : Attribute
    {
        public string GetExpression()
        {
            return "baseCurrency.code";
        }
    }
}
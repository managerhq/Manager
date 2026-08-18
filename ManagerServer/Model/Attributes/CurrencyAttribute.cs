using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class CurrencyAttribute : Attribute
    {
        public string[] path;

        public CurrencyAttribute(string field = null)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(field)) list.Add(field);
            list.Add(nameof(ManagerServer.Model.Currency));
            if (list.Count == 1) list.Add(nameof(ManagerServer.Model.Object.Key));
            this.path = list.ToArray();
        }

        public string GetForeignCurrencyKeyExpression()
        {
            var s = string.Empty;

            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = "this." + e;
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return $"({s} in foreignCurrencies ? {s} : null)";
        }

        public string GetCodeExpression()
        {
            var s = string.Empty;

            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = "this." + e;
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return "(" + s + " in foreignCurrencies ? foreignCurrencies[" + s + "].code : baseCurrency.code)";
        }

        public string GetDecimalPlacesExpression()
        {
            var s = string.Empty;

            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = "this." + e;
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return "(" + s + " in foreignCurrencies ? foreignCurrencies[" + s + "].decimalPlaces : baseCurrency.decimalPlaces)";
        }

        public string GetExchangeRateExpression()
        {
            var s = string.Empty;

            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = "this." + e;
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return "(" + s + " in foreignCurrencies ? foreignCurrencies[" + s + "].exchangeRate : '')";
        }

        public string GetExchangeRateIsInverseExpression()
        {
            var s = string.Empty;

            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = "this." + e;
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return "(" + s + " in foreignCurrencies ? foreignCurrencies[" + s + "].exchangeRateIsInverse : '')";
        }
    }
}

using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    public abstract class Currency : NamedObject, IComparable
    {
        public int GetDecimalPlaces()
        {
            var value = GetDecimalPlacesRawValue();
            return value is int v && v >= 0 && v <= 10 ? v : 2;
        }

        public decimal Round(decimal value)
        {
            var decimalPlaces = GetDecimalPlaces();
            var output = Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
            switch (decimalPlaces)
            {
                case 2: output += 0.00m; break;
                case 3: output += 0.000m; break;
                case 4: output += 0.0000m; break;
                case 5: output += 0.00000m; break;
                case 6: output += 0.000000m; break;
                case 7: output += 0.0000000m; break;
                case 8: output += 0.00000000m; break;
            }

            return output;
        }

        public string GetDisplayName()
        {
            return GetCode() + " - " + GetName();
        }

        public virtual decimal GetBaseAmount(decimal currencyAmount, decimal exchangeRate, bool isExchangeRateInverse, BaseCurrency baseCurrency)
        {
            return currencyAmount;
        }

        public string DisplayCode { get { return GetCode(); }}
        public string DisplayPrefix { get { return GetPrefix(); } }
        public string DisplaySuffix { get { return GetSuffix(); } }

        protected abstract int? GetDecimalPlacesRawValue();
        public abstract string GetCode();
        public abstract string GetPrefix();
        public abstract string GetSuffix();

        int IComparable.CompareTo(object obj) => 0;
    }
}

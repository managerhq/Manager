using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class AppendCurrencyAttribute : Attribute
    {
        public string[] path;

        public AppendCurrencyAttribute(params string[] path)
        {
            var list = new List<string>(path);
            list.Add(nameof(ManagerServer.Model.Currency));
            if (list.Count == 1) list.Add(nameof(ManagerServer.Model.Object.Key));
            this.path = list.ToArray();
        }

        public string GetExpression()
        {
            var s = string.Empty;

            foreach (var e in path)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    s = e;
                }
                else
                {
                    s = "(" + s + " || {})." + e;
                }
            }
            return "(" + s + " in foreignCurrencies ? foreignCurrencies[" + s + "].code : baseCurrency.code)";
        }
    }
}
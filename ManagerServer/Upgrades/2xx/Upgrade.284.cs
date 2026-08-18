using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade284(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var salesQuotes = objects.OfType<ManagerServer.Model.SalesQuote>().Where(x => !string.IsNullOrWhiteSpace(x.Reference)).GroupBy(x => x.Reference).ToDictionary(x => x.Key, x => x.ToArray());
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.QuoteNumber) && x.SalesQuote == null).ToArray())
            {
                if (salesQuotes.ContainsKey(e.QuoteNumber))
                {
                    var salesQuote = salesQuotes[e.QuoteNumber].FirstOrDefault(x => x.Customer == e.Customer);
                    if (salesQuote != null)
                    {
                        e.SalesQuote = salesQuote.Key;
                        list.Add(e);
                    }
                }
            }
            return list;
        }
    }
}

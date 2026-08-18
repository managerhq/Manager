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
        private static async Task<IEnumerable<Model.Object>> Upgrade81(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var salesQuoteTemplate = objects.OfType<Model.Obsolete.Obsolete26.SalesQuoteTemplate26>().SingleOrDefault(x => x.Key == new Guid("2903bbf5-6c43-4fbf-9eef-9b239b784f87"));
            var notes = "Prices valid for 30 days from date on this quote";
            if (salesQuoteTemplate != null && !string.IsNullOrWhiteSpace(salesQuoteTemplate.Obsolete_Notes)) notes = salesQuoteTemplate.Obsolete_Notes;
            foreach (var e in objects.OfType<Model.SalesQuote>().ToArray())
            {
                e.Obsolete_Notes = notes;
                list.Add(e);
            }
            return list;
        }
    }
}

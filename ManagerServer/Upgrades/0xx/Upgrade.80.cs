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
        private static async Task<IEnumerable<Model.Object>> Upgrade80(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var salesInvoiceTemplate = objects.OfType<Model.Obsolete.Obsolete26.SalesInvoiceTemplate26>().SingleOrDefault(x => x.Key == new Guid("55c81ff0-2892-41fb-bff8-3fef6debba85"));
            if (salesInvoiceTemplate != null && !string.IsNullOrWhiteSpace(salesInvoiceTemplate.Obsolete_Notes))
            {
                foreach (var e in objects.OfType<Model.SalesInvoice>().ToArray())
                {
                    e.Obsolete_Notes = salesInvoiceTemplate.Obsolete_Notes;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

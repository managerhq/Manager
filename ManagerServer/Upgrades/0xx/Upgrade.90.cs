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
        private static async Task<IEnumerable<Model.Object>> Upgrade90(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var o = objects.OfType<Model.Obsolete.Obsolete26.SalesInvoiceTemplate26>().SingleOrDefault(x => x.Key == new Guid("55c81ff0-2892-41fb-bff8-3fef6debba85"));
            if (o != null) list.Add(new Model.Obsolete.Obsolete36.SalesInvoiceDefaultNotes36() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete36.SalesInvoiceDefaultNotes36)), Value = o.Obsolete_Notes });
            var o2 = objects.OfType<Model.Obsolete.Obsolete26.SalesQuoteTemplate26>().SingleOrDefault(x => x.Key == new Guid("2903bbf5-6c43-4fbf-9eef-9b239b784f87"));
            if (o2 != null) list.Add(new Model.Obsolete.Obsolete36.SalesQuoteDefaultNotes36() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete36.SalesQuoteDefaultNotes36)), Value = o2.Obsolete_Notes });
            return list;
        }
    }
}

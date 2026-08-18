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
        private static async Task<IEnumerable<Model.Object>> Upgrade145(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var obsoleteItems = new HashSet<Guid>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete24.SalesInvoiceItem24>().ToArray()) obsoleteItems.Add(e.Key);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete24.PurchaseInvoiceItem24>().ToArray()) obsoleteItems.Add(e.Key);

            var fix = new Func<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine[], bool>(lines =>
            {
                var dirty = false;
                if (lines != null)
                {
                    foreach (var e2 in lines)
                    {
                        if (e2 == null) continue;
                        if (e2.Item.HasValue && obsoleteItems.Contains(e2.Item.Value))
                        {
                            dirty = true;
                            e2.Item = null;
                        }
                    }
                }
                return dirty;
            });

            var list = new List<Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().ToArray()) if (fix(e.Obsolete_Lines2)) list.Add(e);

            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.CreditNote>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.SalesQuote>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.SalesOrder>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.DeliveryNote>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);

            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.DebitNote>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseOrder>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);

            return list;
        }
    }
}

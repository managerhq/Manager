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
        private static async Task<IEnumerable<Model.Object>> Upgrade69(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var worker = new Action<Model.Object, Model.Obsolete.Obsolete76.TransactionLine[]>((o, lines) =>
            {
                if (lines == null) return;
                var dirty = false;
                foreach (var e in lines)
                {
                    if (e.Obsolete_Discount.HasValue)
                    {
                        e.Discount = e.Obsolete_Discount.Value;
                        dirty = true;
                    }
                }
                if (dirty) list.Add(o);
            });

            foreach (var e in objects.OfType<Model.SalesInvoice>().ToArray()) worker(e, e.Obsolete_Lines);
            foreach (var e in objects.OfType<Model.CreditNote>().ToArray()) worker(e, e.Obsolete_Lines);
            foreach (var e in objects.OfType<Model.SalesQuote>().ToArray()) worker(e, e.Obsolete_Lines);
            foreach (var e in objects.OfType<Model.PurchaseOrder>().ToArray()) worker(e, e.Obsolete_Lines);
            foreach (var e in objects.OfType<Model.PurchaseInvoice>().ToArray()) worker(e, e.Obsolete_Lines);
            return list;
        }
    }
}

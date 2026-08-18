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
        private static async Task<IEnumerable<Model.Object>> Upgrade167(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.CreditNote>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.DebitNote>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseOrder>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SalesOrder>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SalesQuote>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.RecurringSalesInvoice>().Where(x => x.Obsolete_Lines != null && x.Obsolete_Lines.Any(y => y.Discount.HasValue && y.Discount.Value != 0m)).ToArray())
            {
                e.Discount = true;
                e.DiscountType = DiscountType.Percentage;
                list.Add(e);
            }
            return list.ToArray();
        }
    }
}

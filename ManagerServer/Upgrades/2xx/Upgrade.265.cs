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
        private static async Task<IEnumerable<Model.Object>> Upgrade265(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.SalesOrder>())
            {
                if (e.Lines == null && e.Obsolete_Lines != null)
                {
                    e.Lines = e.Obsolete_Lines.Where(x => x != null).Select(x => new SalesOrder.Line()
                    {
                        Item = x.Item,
                        LineDescription = x.Description,
                        CustomFields = x.CustomFields,
                        SalesUnitPrice = x.Amount ?? 0m,
                        Qty = x.Qty,
                        DiscountAmount = x.DiscountAmount ?? 0m,
                        DiscountPercentage = x.Discount ?? 0m,
                        TaxCode = x.TaxCode
                    }).ToArray();
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

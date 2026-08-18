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
        private static async Task<IEnumerable<Model.Object>> Upgrade318(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Receipt>().Where(x => x.Lines != null && x.Lines.Any(y => y.Qty.HasValue)))
            {
                foreach (var e2 in e.Lines)
                {
                    e2.SalesUnitPrice = e2.Amount;
                }
                e.QuantityColumn = true;
                e.UnitPriceColumn = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Payment>().Where(x => x.Lines != null && x.Lines.Any(y => y.Qty.HasValue)))
            {
                foreach (var e2 in e.Lines)
                {
                    e2.PurchaseUnitPrice = e2.Amount;
                }
                e.QuantityColumn = true;
                e.UnitPriceColumn = true;
                list.Add(e);
            }
            return list;
        }
    }
}

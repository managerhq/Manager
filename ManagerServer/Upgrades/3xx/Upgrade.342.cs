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
        private static async Task<IEnumerable<Model.Object>> Upgrade342(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.InventoryItem>())
            {
                if (!string.IsNullOrWhiteSpace(e.DefaultLineDescription))
                {
                    e.HasDefaultLineDescription = true;
                    list.Add(e);
                }
                if (e.DefaultTaxCode.HasValue)
                {
                    e.HasDefaultTaxCode = true;
                    list.Add(e);
                }
                if (e.DefaultSalesUnitPrice != 0m)
                {
                    e.HasDefaultSalesUnitPrice = true;
                    list.Add(e);
                }
                if (e.DefaultPurchaseUnitPrice != 0m)
                {
                    e.HasDefaultPurchaseUnitPrice = true;
                    list.Add(e);
                }
            }
            return list.Distinct().ToList();
        }
    }
}

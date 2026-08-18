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
        private static async Task<IEnumerable<Model.Object>> Upgrade395(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.InventoryUnitCost>().Where(x => x.InventoryItem.HasValue).GroupBy(x => x.InventoryItem.Value))
            {
                var list2 = e.OrderBy(x => x.Date).ToArray();
                for (int i = 0; i < list2.Length; i++)
                {
                    if (i == 0) continue;
                    if (list2[i].UnitCost == list2[i - 1].UnitCost)
                    {
                        list.Add(new ManagerServer.Model.Obsolete.ObsoleteSingleton() { Key = list2[i].Key });
                    }
                }
            }

            return list;
        }
    }
}

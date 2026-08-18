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
        private static async Task<IEnumerable<Model.Object>> Upgrade253(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.InventoryKit>())
            {
                if (e.BillOfMaterials != null)
                {
                    foreach (var e2 in e.BillOfMaterials)
                    {
                        e2.Qty = e2.Obsolete_Qty ?? 1m;
                    }
                    list.Add(e);
                }
            }
            return list.Distinct();
        }
    }
}

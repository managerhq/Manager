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
        private static async Task<IEnumerable<Model.Object>> Upgrade193(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.InventoryItem>().Where(x => x.Obsolete_StartingBalanceQty2 > 0m))
            {
                e.Obsolete_StartingBalance2 = new Model.InventoryItem.Obsolete_StartingBalanceQuantity[] { new Model.InventoryItem.Obsolete_StartingBalanceQuantity() { InventoryLocation = e.Obsolete_StartingBalanceInventoryLocation, Qty = e.Obsolete_StartingBalanceQty2 } };
                list.Add(e);
            }
            return list;
        }
    }
}

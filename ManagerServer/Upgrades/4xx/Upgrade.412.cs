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
        private static async Task<IEnumerable<Model.Object>> Upgrade412(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var inventoryOnHand = objects.Single<BalanceSheetInventoryOnHandAccount>();
            var negativeInventoryClearing = objects.Single<BalanceSheetNegativeInventoryClearing>();

            if (!negativeInventoryClearing.Group.HasValue) negativeInventoryClearing.Group = inventoryOnHand.Group;
            if (negativeInventoryClearing.Position == 0) negativeInventoryClearing.Position = inventoryOnHand.Position + 1;

            return [negativeInventoryClearing];
        }
    }
}

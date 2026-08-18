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
        private static async Task<IEnumerable<Model.Object>> Upgrade75(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete18.GeneralLedgerAccount18>().Where(x => !x.Obsolete_HasOpeningBalance).ToArray())
            {
                e.StartingBalance = 0m;
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.InventoryItem>().Where(x => !x.Obsolete_HasOpeningBalance).ToArray())
            {
                e.Obsolete_StartingBalanceQty2 = 0m;
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.FixedAsset>().Where(x => !x.Obsolete_HasOpeningBalance).ToArray())
            {
                e.Obsolete_StartingBalanceAccumulatedDepreciation2 = 0m;
                e.Obsolete_StartingBalanceAcquisitionCost2 = 0m;
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.CapitalAccount>().Where(x => !x.Obsolete_HasOpeningBalance).ToArray())
            {
                e.Obsolete_StartingBalanceAmount2 = 0m;
                list.Add(e);
            }
            return list;
        }
    }
}

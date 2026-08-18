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
        private static async Task<IEnumerable<Model.Object>> Upgrade374(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<FixedAsset>().Where(x => x.Obsolete_StartingBalanceAcquisitionCost2 != 0m || x.Obsolete_StartingBalanceAccumulatedDepreciation2 != 0m).ToArray())
            {
                e.Obsolete_StartingBalance2 = true;
                list.Add(e);
            }
            return list;
        }
    }
}

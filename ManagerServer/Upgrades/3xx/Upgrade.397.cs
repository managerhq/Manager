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
        private static async Task<IEnumerable<Model.Object>> Upgrade397(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            if (objects.OfType<InventoryItem>().Length > 0)
            {
                return [new ObsoleteInventoryCostCalculation()
                {
                    Key = new Guid("6d6d2b53-eb10-461b-90f6-eb2fa0609521"),
                    Enabled = true
                }];
            }
            return null;
        }
    }
}

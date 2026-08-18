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
        private static async Task<IEnumerable<Model.Object>> Upgrade383(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            /*
            if (!objects.OfType<Manager.Model.InventoryItem>().Any()) return null;

            var list = new List<Manager.Model.Object>();
            list.Add(new InventoryAutomaticRevaluation() { Key = Manager.Model.Object.GetGuidByType(typeof(InventoryAutomaticRevaluation)), Enabled = true });
            return list;
            */
            return null;
        }
    }
}

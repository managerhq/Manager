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
        private static async Task<IEnumerable<Model.Object>> Upgrade393(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            foreach (var e in objects.OfType<JournalEntry>().Where(x => x.Obsolete_InventoryWriteOff != null))
            {
                e.Obsolete_InventoryWriteOff.Key = e.Key;
                list.Add(e.Obsolete_InventoryWriteOff);
            }

            return list;
        }
    }
}

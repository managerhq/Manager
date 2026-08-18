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
        private static async Task<IEnumerable<Model.Object>> Upgrade88(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Customer>().Where(x => x.Obsolete_StartingBalance2 != 0m).ToArray())
            {
                e.Obsolete_StartingBalance2 = e.Obsolete_StartingBalance2 * -1;
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Supplier>().Where(x => x.Obsolete_StartingBalance2 != 0m).ToArray())
            {
                e.Obsolete_StartingBalance2 = e.Obsolete_StartingBalance2 * -1;
                list.Add(e);
            }
            return list;
        }
    }
}

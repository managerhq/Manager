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
        private static async Task<IEnumerable<Model.Object>> Upgrade347(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.UserPermissions>())
            {
                if (e.Obsolete_Reports2 != null && e.Obsolete_Reports2.Any(x => x.Value.HasValue) && e.Namespaces != null && !e.Namespaces.ContainsKey("Reports"))
                {
                    e.Namespaces.Add("Reports", true);
                    list.Add(e);
                }
                if (e.Obsolete_Settings2 != null && e.Obsolete_Settings2.Any(x => x.Value.HasValue) && e.Namespaces != null && !e.Namespaces.ContainsKey("Settings"))
                {
                    e.Namespaces.Add("Settings", true);
                    list.Add(e);
                }
            }
            return list.Distinct();
        }
    }
}

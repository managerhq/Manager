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
        private static async Task<IEnumerable<Model.Object>> Upgrade346(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.UserPermissions>())
            {
                e.Namespaces = new Dictionary<string, bool>();
                e.Namespaces2 = new Dictionary<string, PermittedActions?>();

                if (e.Obsolete_FullAccess)
                {
                    e.AccessType = UserPermissionsAccessType.FullAccess;
                }
                if (e.Obsolete_Tabs2 != null)
                {
                    foreach (var e2 in e.Obsolete_Tabs2)
                    {
                        e.Namespaces.Add(e2.Key, e2.Value.HasValue);
                        e.Namespaces2.Add(e2.Key, e2.Value);
                    }
                }
                if (e.Obsolete_Settings2 != null)
                {
                    foreach (var e2 in e.Obsolete_Settings2)
                    {
                        e.Namespaces.Add("Settings." + e2.Key, e2.Value.HasValue);
                        e.Namespaces2.Add("Settings." + e2.Key, e2.Value);
                    }
                }
                if (e.Obsolete_Reports2 != null)
                {
                    foreach (var e2 in e.Obsolete_Reports2)
                    {
                        e.Namespaces.Add("Reports." + e2.Key, e2.Value.HasValue);
                        e.Namespaces2.Add("Reports." + e2.Key, e2.Value);
                    }
                }

                list.Add(e);
            }
            return list;
        }
    }
}

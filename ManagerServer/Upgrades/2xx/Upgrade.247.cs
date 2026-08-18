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
        private static async Task<IEnumerable<Model.Object>> Upgrade247(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.UserPermissions>();
            foreach (var e in objects.OfType<ManagerServer.Model.UserPermissions>())
            {
                if (e.Obsolete_Tabs != null) e.Obsolete_Tabs2 = e.Obsolete_Tabs.ToDictionary(x => x, x => (PermittedActions?)e.Obsolete_PermittedActions);
                if (e.Obsolete_Reports != null) e.Obsolete_Reports2 = e.Obsolete_Reports.ToDictionary(x => x, x => (PermittedActions?)e.Obsolete_PermittedActions);
                if (e.Obsolete_Settings != null) e.Obsolete_Settings2 = e.Obsolete_Settings.ToDictionary(x => x, x => (PermittedActions?)e.Obsolete_PermittedActions);
                list.Add(e);
            }
            return list;
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade352(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var tabs = objects.SingleOrDefault<ManagerServer.Model.Tabs>(ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Tabs)));
            if (tabs != null && tabs.Obsolete_BillableExpenses)
            {
                list.Add(new BillableExpenses() { Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BillableExpenses)), Enabled = true });
            }
            return list;
        }
    }
}

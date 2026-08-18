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
        private static async Task<IEnumerable<Model.Object>> Upgrade138(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete23.ControlAccountForCashAccounts23>().ToArray())
            {
                list.Add(new ManagerServer.Model.BalanceSheetAccount() { Key = e.Key, Obsolete_ControlAccount = true, Obsolete_Code = e.Code, Group = e.Group, Name = e.Name });
            }
            return list;
        }
    }
}

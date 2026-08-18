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
        private static async Task<IEnumerable<Model.Object>> Upgrade287(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.UserPermissions>())
            {
                if (e.Obsolete_Tabs2 == null) continue;
                if (e.Obsolete_Tabs2.ContainsKey("BankAccounts") && !e.Obsolete_Tabs2.ContainsKey(nameof(ManagerServer.Model.Tabs.BankAndCashAccounts)))
                {
                    e.Obsolete_Tabs2.Add(nameof(ManagerServer.Model.Tabs.BankAndCashAccounts), e.Obsolete_Tabs2["BankAccounts"]);
                }
                if (e.Obsolete_Tabs2.ContainsKey("CashAccounts") && !e.Obsolete_Tabs2.ContainsKey(nameof(ManagerServer.Model.Tabs.BankAndCashAccounts)))
                {
                    e.Obsolete_Tabs2.Add(nameof(ManagerServer.Model.Tabs.BankAndCashAccounts), e.Obsolete_Tabs2["CashAccounts"]);
                }
                if (e.Obsolete_CashAccounts != null && e.Obsolete_CashAccounts.Length > 0)
                {
                    var bankAndCashAccounts = new List<Guid>(e.Obsolete_CashAccounts);
                    if (e.BankAndCashAccounts != null && e.BankAndCashAccounts.Length > 0) bankAndCashAccounts.AddRange(e.BankAndCashAccounts);
                    e.BankAndCashAccounts = bankAndCashAccounts.Distinct().ToArray();
                }
            }
            return list;
        }
    }
}

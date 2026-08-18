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
        private static async Task<IEnumerable<Model.Object>> Upgrade172(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete67.BankRule>().ToArray())
            {
                if (e.Obsolete_GeneralLedgerAccount == ManagerServer.Model.Master.AccountKeys.AccountsReceivable)
                {
                    e.Obsolete_GeneralLedgerAccount = e.Obsolete_Customer;
                    list.Add(e);
                }
                else if (e.Obsolete_GeneralLedgerAccount == ManagerServer.Model.Master.AccountKeys.AccountsPayable)
                {
                    e.Obsolete_GeneralLedgerAccount = e.Obsolete_Supplier;
                    list.Add(e);
                }
                else if (e.Obsolete_GeneralLedgerAccount == ManagerServer.Model.Master.AccountKeys.EmployeeClearingAccount)
                {
                    e.Obsolete_GeneralLedgerAccount = e.Obsolete_Employee;
                    list.Add(e);
                }
                else if (e.Obsolete_GeneralLedgerAccount == ManagerServer.Model.Master.AccountKeys.CapitalAccounts)
                {
                    e.Obsolete_GeneralLedgerAccount = e.Obsolete_CapitalAccount;
                    list.Add(e);
                }
            }
            return list.ToArray();
        }
    }
}

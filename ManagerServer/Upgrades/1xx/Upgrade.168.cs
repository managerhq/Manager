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
        private static async Task<IEnumerable<Model.Object>> Upgrade168(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => !x.Obsolete_IsBankAccount).ToArray())
            {
                var cashAccount = new ManagerServer.Model.Obsolete.Obsolete78.CashAccount() { Code = e.Code, Currency = e.Currency, Inactive = e.Inactive, Key = e.Key, Name = e.Name, Obsolete_HasStartingBalance = e.Obsolete_HasStartingBalance, StartingBalance = e.Obsolete_StartingBalance2, ControlAccount = e.ControlAccount, CustomFields = e.CustomFields };
                list.Add(cashAccount);
            }
            var cashAtBank = objects.OfType<ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount>().SingleOrDefault(x => x.Key == ManagerServer.Model.Master.AccountKeys.CashAtBank);
            if (cashAtBank != null) list.Add(new ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount() { Key = ManagerServer.Model.Master.AccountKeys.CashOnHand, Group = cashAtBank.Group });
            return list.ToArray();
        }
    }
}

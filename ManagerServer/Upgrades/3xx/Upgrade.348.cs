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
        private static async Task<IEnumerable<Model.Object>> Upgrade348(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.BankOrCashAccount>())
            {
                e.CanHavePendingTransactions = true;
                if (e.CreditLimit > 0m) e.HasCreditLimit = true;
                list.Add(e);
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete78.CashAccount>())
            {
                list.Add(new BankOrCashAccount()
                {
                    Code = e.Code,
                    ControlAccount = e.ControlAccount,
                    Currency = e.Currency,
                    CustomFields = e.CustomFields,
                    Division = e.Division,
                    Inactive = e.Inactive,
                    Key = e.Key,
                    Name = e.Name,
                    Obsolete_StartingBalance2 = e.StartingBalance,
                    Timestamp = e.Timestamp
                });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete78.ControlAccountForCashAccounts>())
            {
                list.Add(new ControlAccountForBankAccounts()
                {
                    Code = e.Code,
                    Name = e.Name,
                    Group = e.Group,
                    Inactive = e.Inactive,
                    Key = e.Key,
                    Position = e.Position,
                    Timestamp = e.Timestamp
                });
            }

            return list;
        }
    }
}

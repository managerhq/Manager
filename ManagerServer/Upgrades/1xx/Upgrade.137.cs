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
        private static async Task<IEnumerable<Model.Object>> Upgrade137(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete22.CashAccount22>().ToArray())
            {
                list.Add(new ManagerServer.Model.BankOrCashAccount() { Key = e.Key, Obsolete_Type = CashAccountType.CashOnHand, Currency = e.Currency, Name = e.Name, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete22.BankAccount22>().ToArray())
            {
                list.Add(new ManagerServer.Model.BankOrCashAccount() { Key = e.Key, Obsolete_Type = CashAccountType.CashAtBank, Currency = e.Currency, Name = e.Name, Obsolete_HasStartingBalance = e.HasStartingBalance, Obsolete_StartingBalance2 = e.StartingBalance, Obsolete_AccountNumber = e.AccountNumber, Obsolete_FinancialInsitution = e.FinancialInstitution, CreditLimit = e.CreditLimit ?? 0m });
            }
            return list;
        }
    }
}

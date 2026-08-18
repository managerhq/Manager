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
        private static async Task<IEnumerable<Model.Object>> Upgrade170(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            var bankAccounts = objects.OfType<ManagerServer.Model.BankOrCashAccount>().ToDictionary(x => x.Key);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankPayment42>().Where(x => x.Lines != null).Where(x => !x.BankAccount.HasValue || !bankAccounts.ContainsKey(x.BankAccount.Value)).ToArray())
            {
                foreach (var e2 in e.Lines)
                {
                    e2.Obsolete_Amount = e2.Amount;
                    e2.Amount = 0m;
                }
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42>().Where(x => x.Lines != null).Where(x => !x.BankAccount.HasValue || !bankAccounts.ContainsKey(x.BankAccount.Value)).ToArray())
            {
                foreach (var e2 in e.Lines)
                {
                    e2.Obsolete_Amount = e2.Amount;
                    e2.Amount = 0m;
                }
                list.Add(e);
            }

            var cashAccounts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete78.CashAccount>().ToDictionary(x => x.Key);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashPayment43>().Where(x => x.Lines != null).Where(x => !x.CashAccount.HasValue || !cashAccounts.ContainsKey(x.CashAccount.Value)).ToArray())
            {
                foreach (var e2 in e.Lines)
                {
                    e2.Obsolete_Amount = e2.Amount;
                    e2.Amount = 0m;
                }
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43>().Where(x => x.Lines != null).Where(x => !x.CashAccount.HasValue || !cashAccounts.ContainsKey(x.CashAccount.Value)).ToArray())
            {
                foreach (var e2 in e.Lines)
                {
                    e2.Obsolete_Amount = e2.Amount;
                    e2.Amount = 0m;
                }
                list.Add(e);
            }

            return list.ToArray();
        }
    }
}

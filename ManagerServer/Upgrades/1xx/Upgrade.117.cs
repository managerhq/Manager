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
        private static async Task<IEnumerable<Model.Object>> Upgrade117(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var interAccountTransfers = new Guid("b9265ae1-69b1-4bc0-bb91-86b7e667f423");

            var bankAccounts = objects.OfType<Model.Obsolete.Obsolete22.BankAccount22>().ToDictionary(x => x.Key);
            var cashAccounts = objects.OfType<Model.Obsolete.Obsolete22.CashAccount22>().ToDictionary(x => x.Key);

            if (objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Lines != null).SelectMany(x => x.Lines).Any(x => x.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CashAtBank || x.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CashOnHand))
            {
                list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Equity, Name = "Inter-account transfers", Key = interAccountTransfers });
            }

            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;

                foreach (var e2 in e.Obsolete_Lines)
                {
                    if (e2.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CashAtBank || e2.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CashOnHand)
                    {
                        dirty = true;
                        var amount = (e2.Debit ?? 0m) - (e2.Credit ?? 0m);
                        if (e2.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CashAtBank && e2.Obsolete_BankAccount.HasValue && bankAccounts.ContainsKey(e2.Obsolete_BankAccount.Value))
                        {
                            e2.Account = interAccountTransfers;
                            e2.Description = bankAccounts[e2.Obsolete_BankAccount.Value].Name;
                            if (amount > 0) list.Add(new Model.Obsolete.Obsolete33.Receipt33() { DebitAccount = e2.Obsolete_BankAccount.Value, Date = e.Date, Key = Guid.CreateVersion7(), Description = e.Narration, Lines = new Model.Obsolete.Obsolete76.TransactionLine[] { new Model.Obsolete.Obsolete76.TransactionLine() { Account = interAccountTransfers, Amount = amount } } });
                            if (amount < 0) list.Add(new Model.Obsolete.Obsolete33.Payment33() { CreditAccount = e2.Obsolete_BankAccount.Value, Date = e.Date, Key = Guid.CreateVersion7(), Description = e.Narration, Lines = new Model.Obsolete.Obsolete76.TransactionLine[] { new Model.Obsolete.Obsolete76.TransactionLine() { Account = interAccountTransfers, Amount = amount * -1 } } });
                        }
                        if (e2.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CashOnHand && e2.Obsolete_CashAccount.HasValue && cashAccounts.ContainsKey(e2.Obsolete_CashAccount.Value))
                        {
                            e2.Account = interAccountTransfers;
                            e2.Description = cashAccounts[e2.Obsolete_CashAccount.Value].Name;
                            if (amount > 0) list.Add(new Model.Obsolete.Obsolete33.Receipt33() { DebitAccount = e2.Obsolete_CashAccount.Value, Date = e.Date, Key = Guid.CreateVersion7(), Description = e.Narration, Lines = new Model.Obsolete.Obsolete76.TransactionLine[] { new Model.Obsolete.Obsolete76.TransactionLine() { Account = interAccountTransfers, Amount = amount } } });
                            if (amount < 0) list.Add(new Model.Obsolete.Obsolete33.Payment33() { CreditAccount = e2.Obsolete_CashAccount.Value, Date = e.Date, Key = Guid.CreateVersion7(), Description = e.Narration, Lines = new Model.Obsolete.Obsolete76.TransactionLine[] { new Model.Obsolete.Obsolete76.TransactionLine() { Account = interAccountTransfers, Amount = amount * -1 } } });
                        }
                    }
                }

                if (dirty)
                {
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

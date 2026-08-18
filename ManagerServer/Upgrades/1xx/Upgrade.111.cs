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
        private static async Task<IEnumerable<Model.Object>> Upgrade111(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var bankAccounts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete22.BankAccount22>().ToDictionary(x => x.Key);
            var cashAccounts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete22.CashAccount22>().ToDictionary(x => x.Key);
            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Lines != null).ToArray())
            {
                var lines = ProtoBuf.Serializer.DeepClone<Model.Obsolete.Obsolete76.TransactionLine[]>(e.Obsolete_Lines).ToList();
                foreach (var e2 in lines)
                {
                    e2.Amount = (e2.Debit ?? 0m) - (e2.Credit ?? 0m);
                    if (e2.Account == Model.Master.AccountKeys.Obsolete_CashAtBank && e2.Obsolete_BankAccount.HasValue && bankAccounts.ContainsKey(e2.Obsolete_BankAccount.Value)) e2.Account = e2.Obsolete_BankAccount.Value;
                    if (e2.Account == Model.Master.AccountKeys.Obsolete_CashOnHand && e2.Obsolete_CashAccount.HasValue && cashAccounts.ContainsKey(e2.Obsolete_CashAccount.Value)) e2.Account = e2.Obsolete_CashAccount.Value;
                }
                var total = lines.Sum(x => x.Amount);
                if (total != 0m)
                {
                    lines.Add(new Model.Obsolete.Obsolete76.TransactionLine() { Amount = total * -1 });
                }

                var splitLines = lines.GroupBy(x => x.Account.HasValue && (bankAccounts.ContainsKey(x.Account.Value) || cashAccounts.ContainsKey(x.Account.Value))).ToDictionary(x => x.Key, x => x.ToList());
                if (!splitLines.ContainsKey(true)) continue;

                if (splitLines[true].Select(x => x.Account.Value).Distinct().Count() == 1 && splitLines.ContainsKey(false) && (splitLines[false].All(x => x.Amount <= 0m) || splitLines[false].All(x => x.Amount >= 0m)))
                {
                    if (splitLines[true][0].Amount > 0)
                    {
                        var receipt = new ManagerServer.Model.Obsolete.Obsolete33.Receipt33()
                        {
                            Date = e.Date,
                            Description = e.Narration,
                            Key = e.Key,
                            Obsolete_JournalEntry = e,
                            Reference = e.Reference,
                            Lines = splitLines[false].ToArray(),
                            DebitAccount = splitLines[true][0].Account.Value,
                        };
                        foreach (var e2 in receipt.Lines) e2.Amount *= -1;
                        list.Add(receipt);
                    }
                    if (splitLines[true][0].Amount < 0)
                    {
                        var payment = new ManagerServer.Model.Obsolete.Obsolete33.Payment33()
                        {
                            Date = e.Date,
                            Description = e.Narration,
                            Key = e.Key,
                            Obsolete_JournalEntry = e,
                            Reference = e.Reference,
                            Lines = splitLines[false].ToArray(),
                            CreditAccount = splitLines[true][0].Account.Value,
                        };
                        list.Add(payment);
                    }
                }
                else if (splitLines[true].Select(x => x.Account.Value).Distinct().Count() == 2 && !splitLines.ContainsKey(false))
                {
                    var transferLines = splitLines[true].GroupBy(x => x.Account.Value).Select(x => new { Account = x.Key, Amount = x.Sum(y => y.Amount) }).OrderBy(x => x.Amount).ToArray();

                    var transfer = new ManagerServer.Model.InterAccountTransfer()
                    {
                        Date = e.Date,
                        Key = e.Key,
                        Description = e.Narration,
                        Reference = e.Reference,
                        CreditAmount = (transferLines[0].Amount ?? 0m) * -1,
                        PaidFrom = transferLines[0].Account,
                        ReceivedIn = transferLines[1].Account,
                        Obsolete_JournalEntry = e
                    };
                    list.Add(transfer);
                }
            }
            return list;
        }
    }
}

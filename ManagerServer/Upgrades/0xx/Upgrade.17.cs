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
        private static async Task<IEnumerable<Model.Object>> Upgrade17(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var associates = objects.OfType<Model.Obsolete.Obsolete02.Associate02>().ToDictionary(x => x.Key);
            var outOfPocketExpenseCreditors = objects.OfType<Model.Obsolete.Obsolete40.OutOfPocketExpensePayor40>().ToDictionary(x => x.Name, x => x.Key);
            var salesInvoices = new HashSet<Guid>(objects.OfType<Model.SalesInvoice>().Select(x => x.Key));
            var purchaseInvoices = new HashSet<Guid>(objects.OfType<Model.PurchaseInvoice>().Select(x => x.Key));
            var bankAccounts = new HashSet<Guid>(objects.OfType<Model.Obsolete.Obsolete22.BankAccount22>().Select(x => x.Key));
            var outOfPocketExpenses = objects.OfType<Model.Obsolete.Obsolete02.OutOfPocketExpense02>().ToArray();
            foreach (var e in outOfPocketExpenses)
            {
                var o = new Model.Obsolete.Obsolete33.Payment33();
                o.Key = e.Key;
                o.Date = e.Date ?? DateTime.MaxValue;
                o.Payee = e.To;
                o.Description = e.Notes;
                if (e.CreditAccount.HasValue && associates.ContainsKey(e.CreditAccount.Value))
                {
                    var name = associates[e.CreditAccount.Value].Name;
                    if (!outOfPocketExpenseCreditors.ContainsKey(name))
                    {
                        var key = Guid.CreateVersion7();
                        list.Add(new Model.Obsolete.Obsolete40.OutOfPocketExpensePayor40() { Key = key, Name = name });
                        outOfPocketExpenseCreditors.Add(name, key);
                    }
                    o.CreditAccount = outOfPocketExpenseCreditors[name];
                }
                if (e.Lines != null)
                {
                    var lines = new List<Model.Obsolete.Obsolete76.TransactionLine>();
                    foreach (var e2 in e.Lines)
                    {
                        var line = new Model.Obsolete.Obsolete76.TransactionLine() { Account = e2.DebitAccount, Description = e.Notes, Amount = e2.Amount ?? 0m, TaxCode = e2.Tax };
                        if (line.Account.HasValue && salesInvoices.Contains(line.Account.Value))
                        {
                            line.Obsolete_SalesInvoice = line.Account.Value;
                            line.Account = Model.Master.AccountKeys.AccountsReceivable;
                        }
                        if (line.Account.HasValue && purchaseInvoices.Contains(line.Account.Value))
                        {
                            line.Obsolete_PurchaseInvoice = line.Account.Value;
                            line.Account = Model.Master.AccountKeys.AccountsPayable;
                        }
                        if (line.Account.HasValue && bankAccounts.Contains(line.Account.Value))
                        {
                            line.Obsolete_BankAccount = line.Account.Value;
                            line.Account = Model.Master.AccountKeys.Obsolete_CashAtBank;
                        }
                        lines.Add(line);
                    }
                    o.Lines = lines.ToArray();
                }
                list.Add(o);
            }
            return list;
        }
    }
}

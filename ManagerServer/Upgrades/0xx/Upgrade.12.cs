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
        private static async Task<IEnumerable<Model.Object>> Upgrade12(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var salesInvoices = new HashSet<Guid>(objects.OfType<Model.SalesInvoice>().Select(x => x.Key));
            var purchaseInvoices = new HashSet<Guid>(objects.OfType<Model.PurchaseInvoice>().Select(x => x.Key));
            var bankAccounts = new HashSet<Guid>(objects.OfType<Model.Obsolete.Obsolete22.BankAccount22>().Select(x => x.Key));
            var receipts = objects.OfType<Model.Obsolete.Obsolete02.Receipt02>().ToArray();
            var list = new List<Model.Object>();
            foreach (var e in receipts)
            {
                var o = new Model.Obsolete.Obsolete33.Receipt33();
                o.Key = e.Key;
                o.Date = e.Date ?? DateTime.MaxValue;
                o.Payer = e.From;
                o.Reference = e.Reference;
                o.Description = e.Notes;
                o.DebitAccount = e.DebitAccount;
                if (e.Lines != null)
                {
                    var lines = new List<Model.Obsolete.Obsolete76.TransactionLine>();
                    foreach (var e2 in e.Lines)
                    {
                        var line = new Model.Obsolete.Obsolete76.TransactionLine() { Account = e2.CreditAccount, Description = e.Notes, Amount = e2.Amount ?? 0m, TaxCode = e2.Tax };
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
            return list.ToArray();
        }
    }
}

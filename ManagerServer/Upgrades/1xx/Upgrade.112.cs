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
        private static async Task<IEnumerable<Model.Object>> Upgrade112(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete14.TransactionExchangeRate14>().ToArray())
            {
                if (e.ExchangeRate == 0m) continue;

                var receipt33 = objects.SingleOrDefault<Model.Obsolete.Obsolete33.Receipt33>(e.Transaction);
                var payment33 = objects.SingleOrDefault<Model.Obsolete.Obsolete33.Payment33>(e.Transaction);
                var journalEntry = objects.SingleOrDefault<Model.JournalEntry>(e.Transaction);
                var expenseClaim = objects.SingleOrDefault<Model.ExpenseClaim>(e.Transaction);

                Model.Obsolete.Obsolete76.TransactionLine[] lines = null;
                if (receipt33 != null) lines = receipt33.Lines;
                if (payment33 != null) lines = payment33.Lines;
                if (journalEntry != null) lines = journalEntry.Obsolete_Lines;
                if (expenseClaim != null) lines = expenseClaim.Obsolete_Lines2;

                if (lines != null)
                {
                    foreach (var e2 in lines)
                    {
                        if (e2 == null) continue;
                        if ((e2.Account == Model.Master.AccountKeys.AccountsPayable && e2.Obsolete_PurchaseInvoice == e.Account) ||
                            (e2.Account == Model.Master.AccountKeys.AccountsReceivable && e2.Obsolete_SalesInvoice == e.Account) ||
                            (e2.Account == Model.Master.AccountKeys.Obsolete_CustomerCredits && e2.Obsolete_Customer == e.Account) ||
                            (e2.Account == Model.Master.AccountKeys.Obsolete_SupplierCredits && e2.Obsolete_Supplier == e.Account) ||
                            (e2.Account == Model.Master.AccountKeys.EmployeeClearingAccount && e2.Obsolete_Employee == e.Account) ||
                            (e2.Account == Model.Master.AccountKeys.Obsolete_CashOnHand && e2.Obsolete_CashAccount == e.Account) ||
                            (e2.Account == Model.Master.AccountKeys.Obsolete_CashAtBank && e2.Obsolete_BankAccount == e.Account))
                        {
                            if (journalEntry != null)
                            {
                                e2.ProposedAccountAmount = Math.Round(((e2.Debit ?? 0m) - (e2.Credit ?? 0m)) * e.ExchangeRate, 2);
                            }
                            else
                            {
                                e2.ProposedAccountAmount = Math.Round((e2.Amount ?? 0m) * e.ExchangeRate, 2);
                            }
                        }
                    }

                    if (receipt33 != null) list.Add(receipt33);
                    if (payment33 != null) list.Add(payment33);
                    if (journalEntry != null) list.Add(journalEntry);
                    if (expenseClaim != null) list.Add(expenseClaim);
                }
            }
            return list;
        }
    }
}

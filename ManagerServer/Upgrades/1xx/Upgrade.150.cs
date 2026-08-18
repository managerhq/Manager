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
        private static async Task<IEnumerable<Model.Object>> Upgrade150(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var fix = new Func<ManagerServer.Model.Obsolete.Obsolete76.TransactionLine[], bool>(lines =>
            {
                var dirty = false;
                if (lines != null)
                {
                    foreach (var e2 in lines)
                    {
                        if (e2 == null) continue;
                        if (e2.Obsolete_Account.HasValue) continue;

                        if (e2.Account == ManagerServer.Model.Master.AccountKeys.InventoryOnHand)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_InventoryItem;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.AccountsPayable)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_PurchaseInvoice;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.AccountsReceivable)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_SalesInvoice;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount)
                        {
                            dirty = true;
                            e2.BillableExpenseCustomer = e2.Obsolete_Customer;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.CapitalAccounts)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_Member;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_CustomerCredits)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_Customer;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.EmployeeClearingAccount)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_Employee;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.ExpenseClaims)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_ExpenseClaimPayer;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.FixedAssets)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_FixedAsset;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.IntangibleAssets)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_IntangibleAsset;
                        }
                        else if (e2.Account == ManagerServer.Model.Master.AccountKeys.Obsolete_SupplierCredits)
                        {
                            dirty = true;
                            e2.Obsolete_Account = e2.Account;
                            e2.Account = e2.Obsolete_Supplier;
                        }
                    }
                }
                return dirty;
            });

            var list = new List<Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().ToArray()) if (fix(e.Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().ToArray()) if (fix(e.Obsolete_Lines2)) list.Add(e);

            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.CreditNote>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);

            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);
            foreach (var e in objects.OfType<ManagerServer.Model.DebitNote>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);

            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().ToArray()) if (fix(e.Obsolete_Lines)) list.Add(e);

            return list;
        }
    }
}

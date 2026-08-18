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
        private static async Task<IEnumerable<Model.Object>> Upgrade267(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>())
            {
                if (e.Lines == null && e.Obsolete_Lines != null)
                {
                    var lines = new List<JournalEntry.Line>();
                    foreach (var e2 in e.Obsolete_Lines)
                    {
                        Guid? customer = e2.BillableExpenseCustomer;
                        Guid? supplier = null;
                        Guid? employee = null;
                        Guid? capitalAccount = null;
                        Guid? specialAccount = null;
                        Guid? inventoryItem = null;
                        Guid? fixedAsset = null;
                        Guid? intangibleAsset = null;
                        Guid? expenseClaimPayer = null;
                        Guid? salesInvoice = e2.BillableExpenseSalesInvoice;
                        Guid? purchaseInvoice = null;

                        Guid? account = e2.Account;
                        if (account.HasValue)
                        {
                            var account2 = objects.SingleOrDefault(account.Value);
                            if (account2 != null)
                            {
                                if (account2 is Customer)
                                {
                                    customer = account;
                                    salesInvoice = e2.Invoice;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsReceivableAccount));
                                }
                                else if (account2 is Supplier)
                                {
                                    supplier = account;
                                    purchaseInvoice = e2.Invoice;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsPayableAccount));
                                }
                                else if (account2 is Employee)
                                {
                                    employee = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetEmployeeClearingAccount));
                                }
                                else if (account2 is CapitalAccount)
                                {
                                    capitalAccount = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetCapitalAccountsAccount));
                                }
                                else if (account2 is SpecialAccount)
                                {
                                    specialAccount = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetSpecialAccountsAccount));
                                }
                                else if (account2 is InventoryItem)
                                {
                                    inventoryItem = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetInventoryOnHandAccount));
                                }
                                else if (account2 is FixedAsset)
                                {
                                    fixedAsset = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetFixedAssetsAtCostAccount));
                                }
                                else if (account2 is IntangibleAsset)
                                {
                                    intangibleAsset = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetIntangibleAssetsAtCostAccount));
                                }
                                else if (account2 is ExpenseClaimsPayer)
                                {
                                    expenseClaimPayer = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetExpenseClaimsAccount));
                                }
                            }
                        }

                        lines.Add(new JournalEntry.Line()
                        {
                            Account = account,
                            AccountsReceivableCustomer = customer,
                            AccountsPayableSupplier = supplier,
                            Employee = employee,
                            CapitalAccount = capitalAccount,
                            SpecialAccount = specialAccount,
                            Obsolete_InventoryItem = inventoryItem,
                            FixedAsset = fixedAsset,
                            IntangibleAsset = intangibleAsset,
                            ExpenseClaimPayer = expenseClaimPayer,
                            AccountsReceivableSalesInvoice = salesInvoice,
                            PurchaseInvoice = purchaseInvoice,
                            LineDescription = e2.Description,
                            TaxCode = e2.TaxCode,
                            Division = e2.TrackingCode,
                            CurrencyAmount = e2.ProposedAccountAmount ?? 0m,
                            Qty = e2.Qty ?? 0m,
                            SubAccount = e2.MemberAccount,
                            Debit = e2.Debit ?? 0m,
                            Credit = e2.Credit ?? 0m
                        });
                    }
                    e.Lines = lines.ToArray();

                    e.HasLineDescription = e.Lines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription));
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.RecurringJournalEntry>())
            {
                if (e.Lines == null && e.Obsolete_Lines != null)
                {
                    var lines = new List<JournalEntry.Line>();
                    foreach (var e2 in e.Obsolete_Lines)
                    {
                        Guid? customer = e2.BillableExpenseCustomer;
                        Guid? supplier = null;
                        Guid? employee = null;
                        Guid? capitalAccount = null;
                        Guid? specialAccount = null;
                        Guid? inventoryItem = null;
                        Guid? fixedAsset = null;
                        Guid? intangibleAsset = null;
                        Guid? expenseClaimPayer = null;
                        Guid? salesInvoice = e2.BillableExpenseSalesInvoice;
                        Guid? purchaseInvoice = null;

                        Guid? account = e2.Account;
                        if (account.HasValue)
                        {
                            var account2 = objects.SingleOrDefault(account.Value);
                            if (account2 != null)
                            {
                                if (account2 is Customer)
                                {
                                    customer = account;
                                    salesInvoice = e2.Invoice;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsReceivableAccount));
                                }
                                else if (account2 is Supplier)
                                {
                                    supplier = account;
                                    purchaseInvoice = e2.Invoice;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsPayableAccount));
                                }
                                else if (account2 is Employee)
                                {
                                    employee = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetEmployeeClearingAccount));
                                }
                                else if (account2 is CapitalAccount)
                                {
                                    capitalAccount = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetCapitalAccountsAccount));
                                }
                                else if (account2 is SpecialAccount)
                                {
                                    specialAccount = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetSpecialAccountsAccount));
                                }
                                else if (account2 is InventoryItem)
                                {
                                    inventoryItem = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetInventoryOnHandAccount));
                                }
                                else if (account2 is FixedAsset)
                                {
                                    fixedAsset = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetFixedAssetsAtCostAccount));
                                }
                                else if (account2 is IntangibleAsset)
                                {
                                    intangibleAsset = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetIntangibleAssetsAtCostAccount));
                                }
                                else if (account2 is ExpenseClaimsPayer)
                                {
                                    expenseClaimPayer = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetExpenseClaimsAccount));
                                }
                            }
                        }

                        lines.Add(new JournalEntry.Line()
                        {
                            Account = account,
                            AccountsReceivableCustomer = customer,
                            AccountsPayableSupplier = supplier,
                            Employee = employee,
                            CapitalAccount = capitalAccount,
                            SpecialAccount = specialAccount,
                            Obsolete_InventoryItem = inventoryItem,
                            FixedAsset = fixedAsset,
                            IntangibleAsset = intangibleAsset,
                            ExpenseClaimPayer = expenseClaimPayer,
                            AccountsReceivableSalesInvoice = salesInvoice,
                            PurchaseInvoice = purchaseInvoice,
                            LineDescription = e2.Description,
                            TaxCode = e2.TaxCode,
                            Division = e2.TrackingCode,
                            CurrencyAmount = e2.ProposedAccountAmount ?? 0m,
                            Qty = e2.Qty ?? 0m,
                            SubAccount = e2.MemberAccount,
                            Debit = e2.Debit ?? 0m,
                            Credit = e2.Credit ?? 0m
                        });
                    }
                    e.Lines = lines.ToArray();

                    e.HasLineDescription = e.Lines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription));
                    list.Add(e);
                }
            }

            return list;
        }
    }
}

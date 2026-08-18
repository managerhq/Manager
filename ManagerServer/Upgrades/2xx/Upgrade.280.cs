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
        private static async Task<IEnumerable<Model.Object>> Upgrade280(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment>())
            {
                if (e.Lines != null)
                {
                    var paymentLines = new List<Payment.Line>();
                    var receiptLines = new List<Receipt.Line>();

                    foreach (var e2 in e.Lines)
                    {
                        Guid? customer = null;
                        Guid? supplier = null;
                        Guid? employee = null;
                        Guid? capitalAccount = null;
                        Guid? specialAccount = null;
                        Guid? fixedAsset = null;
                        Guid? intangibleAsset = null;
                        Guid? expenseClaimPayer = null;
                        Guid? salesInvoice = null;
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
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsReceivableAccount));
                                    salesInvoice = e2.Invoice;
                                }
                                else if (account2 is Supplier)
                                {
                                    supplier = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsPayableAccount));
                                    purchaseInvoice = e2.Invoice;
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

                        paymentLines.Add(new Payment.Line()
                        {
                            Account = account,
                            BillableExpenseCustomer = e2.BillableExpenseCustomer,
                            BillableExpenseSalesInvoice = e2.BillableExpenseSalesInvoice,
                            AccountsReceivableCustomer = customer,
                            AccountsReceivableSalesInvoice = salesInvoice,
                            AccountsPayableSupplier = supplier,
                            PurchaseInvoice = purchaseInvoice,
                            CurrencyAmount = e2.ProposedAccountAmount ?? 0m,
                            LineDescription = e2.Description,
                            Item = e2.Item,
                            Qty = e2.Qty,
                            Amount = e2.Amount ?? 0m,
                            Employee = employee,
                            CapitalAccount = capitalAccount,
                            SpecialAccount = specialAccount,
                            FixedAsset = fixedAsset,
                            IntangibleAsset = intangibleAsset,
                            ExpenseClaimsPayer = expenseClaimPayer,
                            TaxCode = e2.TaxCode,
                            Division = e2.TrackingCode,
                            SubAccount = e2.MemberAccount
                        });

                        receiptLines.Add(new Receipt.Line()
                        {
                            Account = account,
                            BillableExpenseCustomer = e2.BillableExpenseCustomer,
                            BillableExpenseSalesInvoice = e2.BillableExpenseSalesInvoice,
                            AccountsReceivableCustomer = customer,
                            AccountsReceivableSalesInvoice = salesInvoice,
                            AccountsPayableSupplier = supplier,
                            PurchaseInvoice = purchaseInvoice,
                            CurrencyAmount = e2.ProposedAccountAmount ?? 0m,
                            LineDescription = e2.Description,
                            Item = e2.Item,
                            Qty = e2.Qty,
                            Amount = e2.Amount ?? 0m,
                            Employee = employee,
                            CapitalAccount = capitalAccount,
                            SpecialAccount = specialAccount,
                            FixedAsset = fixedAsset,
                            IntangibleAsset = intangibleAsset,
                            ExpenseClaimsPayer = expenseClaimPayer,
                            TaxCode = e2.TaxCode,
                            Division = e2.TrackingCode,
                            SubAccount = e2.MemberAccount
                        });
                    }

                    if (e.Type == Model.Obsolete.Obsolete66.ReceiptOrPaymentType.Receipt)
                    {
                        list.Add(new Receipt()
                        {
                            Key = e.Key,
                            Date = e.Date,
                            Description = e.Description,
                            BankClearDate = e.BankClearDate,
                            Obsolete_Status = e.BankClearStatus,
                            HasLineDescription = receiptLines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription)),
                            Obsolete_AmountsIncludeTax = e.AmountsIncludeTax,
                            Contact = e.Contact,
                            Customer = e.Customer,
                            Supplier = e.Supplier,
                            CustomFields = e.CustomFields,
                            CustomTheme = e.CustomTheme,
                            InventoryLocation = e.InventoryLocation,
                            Lines = receiptLines.ToArray(),
                            HasReceiptCustomTitle = e.HasReceiptCustomTitle,
                            ReceiptCustomTitle = e.ReceiptCustomTitle,
                            Obsolete_ReceiptOrPayment = e,
                            PaidBy = e.PayerPayeeType ?? PayerPayeeType.Other,
                            Reference = e.Reference,
                            ReceivedIn = e.BankAccount
                        });
                    }
                    else
                    {
                        list.Add(new Payment()
                        {
                            Key = e.Key,
                            Date = e.Date,
                            Description = e.Description,
                            BankClearDate = e.BankClearDate,
                            Obsolete_Status = e.BankClearStatus,
                            HasLineDescription = receiptLines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription)),
                            Obsolete_AmountsIncludeTax = e.AmountsIncludeTax,
                            Contact = e.Contact,
                            Customer = e.Customer,
                            Supplier = e.Supplier,
                            CustomFields = e.CustomFields,
                            CustomTheme = e.CustomTheme,
                            InventoryLocation = e.InventoryLocation,
                            Lines = paymentLines.ToArray(),
                            HasPaymentCustomTitle = e.HasPaymentCustomTitle,
                            PaymentCustomTitle = e.PaymentCustomTitle,
                            Obsolete_ReceiptOrPayment = e,
                            Payee = e.PayerPayeeType ?? PayerPayeeType.Other,
                            Reference = e.Reference,
                            PaidFrom = e.BankAccount
                        });
                    }
                }
            }

            return list;
        }
    }
}

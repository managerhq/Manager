using ManagerServer.Model;
using System;
using System.Linq;
using ProtoBuf;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{
    [ProtoContract]
    [Title(nameof(Strings.Payment), nameof(Strings.Edit))]
    [Guide("The `Payment` form records money leaving your business through bank accounts or cash accounts. Use this form to document all outgoing transactions for accurate financial tracking and reporting.")]
    [Guide("Common payment types include purchases from suppliers, employee wages, tax payments to government agencies, rent, utilities, and other business expenses. The form provides specialized features for different payment scenarios.")]
    [Header("Recording a Payment")]
    [Guide("To record a payment, start by selecting the bank or cash account from which the money will be paid. Enter the date when the payment was made or will be made. Then specify who received the payment by selecting the payee type and the specific recipient.")]
    [Guide("For supplier payments, the system automatically displays any outstanding purchase invoices. You can select which invoices to pay and specify the amount for each. The form shows the invoice balance to help ensure accurate payment allocation.")]
    [Guide("For other payments, allocate the payment amount to appropriate expense accounts. You can split a single payment across multiple accounts if needed. For example, a utility payment might be split between electricity and water expense accounts.")]
    [Header("Payment Status")]
    [Guide("Mark payments as pending if they haven't cleared your bank account yet. This status helps with bank reconciliation by distinguishing between payments that have been recorded but not yet processed by the bank. Once the payment clears, you can update its status.")]
    [Header("Additional Features")]
    [Guide("The form supports tax tracking by allowing you to specify tax amounts on payment lines. You can also attach supporting documentation such as receipts or invoices to maintain a complete audit trail. Custom fields can be added to capture additional information specific to your business needs.")]
    [Header("Form Fields")]
    [Guide("The payment form contains the following fields:")]
    [Fields(typeof(Payment))]
    internal sealed class PaymentForm : NakedVueForm<ManagerServer.Model.Payment>
    {
        [ProtoMember(1)] public Guid? BankAccount;
        [ProtoMember(2)] public bool? Pending;
        [ProtoMember(3)] public DateTime? Date;
        [ProtoMember(4)] public bool? EmployeeClearingAccount;
        [ProtoMember(5)] public bool? WithholdingTaxPayable;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(Payment form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (BankAccount.HasValue) form.PaidFrom = BankAccount.Value;
                if (Pending.HasValue && Pending.Value) form.Cleared = ManagerServer.Model.Enums.BankAccountClearStatus.OnALaterDate;
                if (Date.HasValue) form.Date = Date.Value;

                if (EmployeeClearingAccount.HasValue && EmployeeClearingAccount.Value)
                {
                    var balances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsEmployeeClearingAccount).GroupBy(x => x.Employee).Select(x => new { Employee = x.Key, Balance = x.Sum(y => y.AccountAmount) * -1m }).Where(x => x.Balance > 0m).ToArray();
                    form.Lines = balances.Select(x => new ManagerServer.Model.Payment.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetEmployeeClearingAccount)), Employee = x.Employee.Key, Amount = x.Balance }).ToArray();
                    form.QuantityColumn = false;
                    form.UnitPriceColumn = false;
                }

                if (source is Payment payment)
                {
                    Copy(source, form);
                }

                if (source is Receipt receipt)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<Payment>(Business, receipt.CustomFields);
                }

                if (source is Payslip payslip)
                {
                    if (payslip.employee.HasValue)
                    {
                        var employee = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Employee>(payslip.employee.Value);
                        if (employee != null)
                        {
                            var currency = employee.Currency;
                            var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);
                            var numberDecimalPlaces = currencies.Get(currency).GetDecimalPlaces();

                            var netPay = 0m;
                            if (payslip.Earnings != null) netPay += payslip.Earnings.Sum(x => Math.Round((x.Units ?? 1m) * x.UnitPrice, numberDecimalPlaces, MidpointRounding.AwayFromZero));
                            if (payslip.Deductions != null) netPay -= payslip.Deductions.Sum(x => x.DeductionAmount);

                            form.Lines = new Payment.Line[] {
                                new Payment.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetEmployeeClearingAccount)), Employee = employee.Key, Amount = netPay }
                            };

                            form.QuantityColumn = false;
                            form.UnitPriceColumn = false;
                        }
                    }
                }

                if (source is Supplier supplier)
                {                    
                    if (WithholdingTaxPayable == true)
                    {
                        var withholdingTaxPayable = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsWithholdingTaxPayablePayable && x.Supplier.Key == supplier.Key).Sum(x => x.AccountAmount)*-1m;
                        form.Lines = new Payment.Line[] {
                                new Payment.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetWithholdingTaxPayableAccount)), WithholdingTaxPayableSupplier = supplier.Key, Amount = withholdingTaxPayable }
                            };
                    }
                    else
                    {
                        form.Payee = ManagerServer.Model.Enums.PayerPayeeType.Supplier;
                        form.Supplier = supplier.Key;

                        var date = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.SupplierStatementsUnpaidInvoices>().GetDate();

                        var purchaseInvoiceBalances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchPurchaseInvoices(new Guid[] { supplier.Key }).Where(x => x.Date <= date && x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier.Key == supplier.Key && x.PurchaseInvoice != null).GroupBy(x => x.PurchaseInvoice).Select(x => new { PurchaseInvoice = x.Key, Balance = x.Sum(y => y.AccountAmount) * -1m }).Where(x => x.Balance != 0m).OrderBy(x => x.PurchaseInvoice.IssueDate).ToArray();
                        form.Lines = purchaseInvoiceBalances.Select(x => new Payment.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsPayableAccount)), AccountsPayableSupplier = supplier.Key, PurchaseInvoice = x.PurchaseInvoice.Key, Amount = x.Balance }).ToArray();
                    }

                    form.QuantityColumn = false;
                    form.UnitPriceColumn = false;
                }

                if (source is PurchaseInvoice purchaseInvoice)
                {
                    form.Payee = ManagerServer.Model.Enums.PayerPayeeType.Supplier;
                    form.Supplier = purchaseInvoice.Supplier;

                    var purchaseInvoiceBalance = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchPurchaseInvoices(purchaseInvoice.Supplier.HasValue ? new Guid[] { purchaseInvoice.Supplier.Value } : null).Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier.Key == purchaseInvoice.Supplier && x.PurchaseInvoice?.Key == purchaseInvoice.Key).Sum(x => x.AccountAmount)*-1m;
                    form.Lines = new Payment.Line[] { new Payment.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetAccountsPayableAccount)), AccountsPayableSupplier = purchaseInvoice.Supplier, PurchaseInvoice = purchaseInvoice.Key, Amount = purchaseInvoiceBalance } };

                    form.QuantityColumn = false;
                    form.UnitPriceColumn = false;
                }
            }
        }
    }
}

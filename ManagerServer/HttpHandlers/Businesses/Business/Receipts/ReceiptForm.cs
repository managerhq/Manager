using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.Receipt), nameof(Strings.Edit))]
    [Guide("The `Receipt` form enables you to record money coming into your business through bank accounts or cash accounts. This form documents all incoming transactions for accurate financial tracking and proper allocation to income accounts.")]
    [Header("Types of Receipts")]
    [Guide("Receipts can be recorded for various types of incoming funds:")]
    [Guide("• **Customer payments** - Money received from customers paying sales invoices")]
    [Guide("• **Interest income** - Interest earned on bank accounts or investments")]
    [Guide("• **Asset sales** - Proceeds from selling fixed assets or investments")]
    [Guide("• **Loans received** - Funds borrowed from banks or other lenders")]
    [Guide("• **Other income** - Any other money coming into your business")]
    [Header("Recording a Receipt")]
    [Guide("To record a receipt, follow these steps:")]
    [Guide("1. Select the bank or cash account where the money was received")]
    [Guide("2. Enter the date when the receipt occurred")]
    [Guide("3. Specify who made the payment using the `Paid by` field")]
    [Guide("4. For customer receipts, the system automatically displays outstanding invoices that can be marked as paid")]
    [Guide("5. For other receipts, allocate the amounts to appropriate income accounts")]
    [Guide("6. Add any applicable tax codes if the receipt includes taxes")]
    [Header("Bank Reconciliation")]
    [Guide("If a receipt hasn't cleared your bank account yet, mark it as `Pending`. This helps with accurate bank reconciliation by distinguishing between recorded receipts and those that have actually cleared your bank. Once the receipt clears, you can update its status during the bank reconciliation process.")]
    [Header("Advanced Features")]
    [Guide("The receipt form supports several advanced features:")]
    [Guide("• **Split receipts** - Allocate a single receipt across multiple income accounts")]
    [Guide("• **Partial payments** - Record partial payments against sales invoices")]
    [Guide("• **Foreign currency** - Handle receipts in different currencies with automatic exchange rate calculations")]
    [Guide("• **Custom fields** - Add custom information specific to your business needs")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.Receipt))]
    internal sealed class ReceiptForm : NakedVueForm<ManagerServer.Model.Receipt>
    {
        [ProtoMember(1)] public Guid? BankAccount;
        [ProtoMember(2)] public bool? Pending;
        [ProtoMember(3)] public DateTime? Date;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(Receipt form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (BankAccount.HasValue) form.ReceivedIn = BankAccount.Value;
                if (Pending.HasValue && Pending.Value) form.Cleared = ManagerServer.Model.Enums.BankAccountClearStatus.OnALaterDate;
                if (Date.HasValue) form.Date = Date.Value;

                if (source is Receipt receipt)
                {
                    Copy(source, form);
                }

                if (source is Payment payment)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<Receipt>(Business, payment.CustomFields);
                }

                if (source is Customer customer)
                {
                    form.PaidBy = ManagerServer.Model.Enums.PayerPayeeType.Customer;
                    form.Customer = customer.Key;

                    var date = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.CustomerStatementsUnpaidInvoices>().GetDate();

                    var salesInvoiceBalances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices(new Guid[] { customer.Key }).Where(x => x.Date <= date && x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer.Key == customer.Key && x.SalesInvoice != null).GroupBy(x => x.SalesInvoice).Select(x => new { SalesInvoice = x.Key, Balance = x.Sum(y => y.AccountAmount) }).Where(x => x.Balance != 0m).OrderBy(x => x.SalesInvoice.IssueDate).ToArray();
                    form.Lines = salesInvoiceBalances.Select(x => new Receipt.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetAccountsReceivableAccount)), AccountsReceivableCustomer = customer.Key, AccountsReceivableSalesInvoice = x.SalesInvoice.Key, Amount = x.Balance }).ToArray();

                    form.QuantityColumn = false;
                    form.UnitPriceColumn = false;
                }

                if (source is SalesInvoice salesInvoice)
                {
                    form.PaidBy = ManagerServer.Model.Enums.PayerPayeeType.Customer;
                    form.Customer = salesInvoice.Customer;

                    var salesInvoiceBalance = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices(salesInvoice.Customer.HasValue ? new Guid[] { salesInvoice.Customer.Value } : null).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer.Key == salesInvoice.Customer && x.SalesInvoice?.Key == salesInvoice.Key).Sum(x => x.AccountAmount);
                    form.Lines = new Receipt.Line[] { new Receipt.Line() { Account = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetAccountsReceivableAccount)), AccountsReceivableCustomer = salesInvoice.Customer, AccountsReceivableSalesInvoice = salesInvoice.Key, Amount = salesInvoiceBalance } };

                    form.QuantityColumn = false;
                    form.UnitPriceColumn = false;
                }
            }
        }
    }
}

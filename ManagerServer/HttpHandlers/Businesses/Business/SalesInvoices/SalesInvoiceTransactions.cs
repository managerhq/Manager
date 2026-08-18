using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerComponents;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoice), nameof(Strings.Transactions))]
    [Guide("This screen displays all payment transactions that have been applied to a specific sales invoice, allowing you to track the invoice's payment history and current balance.")]
    [Guide("The *balance due* shown at the top reflects the remaining amount owed after accounting for all receipts and credit notes applied to this invoice.")]
    [Guide("Each transaction in the list shows the date, reference number, and amount applied from customer receipts. Positive amounts represent payments received, while negative amounts may indicate credit notes or adjustments.")]
    [Guide("To record a new payment against this invoice, click the **New Receipt** button. This will open a receipt form with the invoice pre-selected, making it easy to apply the payment correctly.")]
    internal sealed class SalesInvoiceTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid SalesInvoice;

        protected override HeaderButton GetPrimaryButton()
        {
            return new HeaderButton()
            {
                Text = Strings.NewReceipt,
                Url = new Receipts.ReceiptForm() { Business = Business, Source = SalesInvoice, Referrer = this.ToUrl() }.ToUrl()
            };
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var salesInvoice = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.SalesInvoice>(SalesInvoice);
            if (salesInvoice == null) return null;
            var customer = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Customer>(salesInvoice.Customer);
            if (customer == null) return null;

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .AutomaticallyMatchSalesInvoices(new Guid[] { salesInvoice.Customer.Value })
                .Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer?.Key == salesInvoice.Customer.Value && x.SalesInvoice?.Key == SalesInvoice)
                .ToArray();
        }
    }
}
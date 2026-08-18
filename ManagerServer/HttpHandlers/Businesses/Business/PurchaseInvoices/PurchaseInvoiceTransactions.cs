using System.Linq;
using ManagerServer.Globalization;
using System.Collections.Generic;
using ManagerServer.Attributes;
using ManagerComponents;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseInvoice), nameof(Strings.Transactions))]
    [Guide("The *Purchase Invoice* transactions screen provides a detailed view of all financial activity related to a specific purchase invoice.")]
    [Guide("This screen helps you track payments made against the invoice and monitor the outstanding *balance due*.")]
    [Header("What This Screen Shows")]
    [Guide("For each purchase invoice, you can view:")]
    [Guide("• The original invoice amount and date")]
    [Guide("• All payments made against the invoice")]
    [Guide("• Any adjustments, *credit notes*, or *debit notes* applied")]
    [Guide("• The current *balance due* to the supplier")]
    [Header("Making Payments")]
    [Guide("Click the **New Payment** button to record a payment against this invoice.")]
    [Guide("The payment will automatically be linked to this purchase invoice, reducing the *balance due*.")]
    internal sealed class PurchaseInvoiceTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid PurchaseInvoice;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override HeaderButton GetPrimaryButton()
        {
            return new HeaderButton()
            {
                Text = Strings.NewPayment,
                Url = new Payments.PaymentForm() { Business = Business, Source = PurchaseInvoice, Referrer = this.ToUrl() }.ToUrl()
            };
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var purchaseInvoice = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.PurchaseInvoice>(PurchaseInvoice);
            if (purchaseInvoice == null) return null;
            var supplier = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Supplier>(purchaseInvoice.Supplier);
            if (supplier == null) return null;

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .AutomaticallyMatchPurchaseInvoices(new Guid[] { purchaseInvoice.Supplier.Value })
                .Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier?.Key == purchaseInvoice.Supplier.Value && x.PurchaseInvoice?.Key == PurchaseInvoice)
                .ToArray();
        }
    }
}
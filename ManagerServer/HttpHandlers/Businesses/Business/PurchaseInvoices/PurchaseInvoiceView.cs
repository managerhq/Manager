using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseInvoice))]
    [Guide("The purchase invoice view displays comprehensive details about a purchase invoice received from a supplier.")]
    [Guide("This view shows all invoice information including supplier details, *invoice date*, *due date*, line items, tax amounts, and the *balance due*.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• Edit the invoice by clicking the **Edit** button")]
    [Guide("• Print or save the invoice as a PDF using the **Print** button")]
    [Guide("• Email the invoice directly from the system using the **Email** button")]
    [Guide("• Create a payment against this invoice using the **New Payment** button")]
    [Header("Payment Tracking")]
    [Guide("The view automatically displays any payments already made against this invoice.")]
    [Guide("The *balance due* is calculated automatically based on the invoice total and any payments received.")]
    [Guide("Each payment is shown with its date and amount, making it easy to track the payment history.")]
    [LinkGuide("To learn how to create or edit purchase invoices, see:", typeof(PurchaseInvoiceForm))]
    internal sealed class PurchaseInvoiceView : TransactionView<ManagerServer.Model.PurchaseInvoice>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForPurchaseInvoice>();
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Supplier>(business.SingleOrDefault<PurchaseInvoice>(Key)?.Supplier)?.Email;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Payment), typeof(ManagerServer.Model.Transaction), typeof(ManagerServer.Model.RecurringPurchaseInvoice)];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new PurchaseInvoiceTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
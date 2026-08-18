using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoice))]
    [Guide("The Sales Invoice view displays comprehensive details about a specific sales invoice, including customer information, line items, totals, and payment status.")]
    [Guide("This view provides a complete overview of the invoice as it appears to customers, showing all relevant transaction details and amounts.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions using the buttons at the bottom of the screen:")]
    [Guide("• Click **Edit** to modify the invoice details, line items, or amounts")]
    [Guide("• Click **Email** to send the invoice directly to the customer's email address")]
    [Guide("• Click **Print** to generate a printable version of the invoice")]
    [Guide("• Click **New Receipt** to record a payment against this invoice")]
    [Guide("• Click **Copy To** to duplicate this invoice to a new sales invoice or recurring invoice")]
    [Header("Invoice Status Indicators")]
    [Guide("The view automatically displays the payment status of the invoice:")]
    [Guide("• **Paid in Full** appears in green when the invoice has been completely paid")]
    [Guide("• **Overdue** appears in red when the invoice is past its due date and has an outstanding balance")]
    [Guide("• The *balance due* amount shows how much the customer still owes")]
    [Header("Special Features")]
    [Guide("Additional information may appear based on your invoice settings:")]
    [Guide("• *Early payment discounts* are displayed if configured, showing the reduced amount due if paid by a specific date")]
    [Guide("• *Total amount in words* converts the numeric total to written text for clarity")]
    [Guide("• *Total amount in base currency* shows the equivalent value when dealing with foreign currency invoices")]
    [LinkGuide("To learn about creating and editing sales invoices, see:", typeof(SalesInvoiceForm))]
    internal sealed class SalesInvoiceView : TransactionView<ManagerServer.Model.SalesInvoice>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Receipt), typeof(ManagerServer.Model.Transaction), typeof(ManagerServer.Model.RecurringSalesInvoice)];
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Customer>(business.SingleOrDefault<SalesInvoice>(Key)?.Customer)?.Email;
        }

        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForSalesInvoice>();
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new SalesInvoiceTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
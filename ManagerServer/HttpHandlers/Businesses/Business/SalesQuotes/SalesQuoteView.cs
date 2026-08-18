using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.SalesQuote), nameof(Strings.View))]
    [Guide("The `Sales Quote` view displays the complete details of a sales quotation that has been created for a customer.")]
    [Guide("This view shows all the information contained in the quote, including proposed pricing, quantities, terms, and any custom fields that have been configured.")]
    [Guide("From this view, you can email the quote to your customer, edit the quote details, or copy the quote to create other transactions such as sales orders or sales invoices.")]
    [Guide("The quote will display customer information, line items with descriptions and amounts, and calculated totals based on your settings.")]
    [LinkGuide("To learn how to create or edit quotes, see:", typeof(SalesQuoteForm))]
    internal sealed class SalesQuoteView : TransactionView<ManagerServer.Model.SalesQuote>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForSalesQuote>();
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Customer>(business.SingleOrDefault<SalesQuote>(Key)?.Customer)?.Email;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction), typeof(ManagerServer.Model.RecurringSalesQuote)];
        }
    }
}
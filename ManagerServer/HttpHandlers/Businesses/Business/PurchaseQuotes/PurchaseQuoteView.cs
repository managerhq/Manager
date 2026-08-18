using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseQuote))]
    [Guide("The *purchase quote* view displays detailed information about a quote received from a supplier.")]
    [Guide("From this view, you can edit the quote, print it, email it to others, or convert it to a **Purchase Order** when you decide to proceed with the purchase.")]
    [LinkGuide("For more information, see:", typeof(PurchaseQuoteForm))]
    internal sealed class PurchaseQuoteView : TransactionView<ManagerServer.Model.PurchaseQuote>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction)];
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Supplier>(business.SingleOrDefault<PurchaseQuote>(Key)?.Supplier)?.Email;
        }

        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForPurchaseQuote>();
        }
    }
}
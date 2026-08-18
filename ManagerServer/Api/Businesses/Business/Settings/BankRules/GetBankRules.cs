using ManagerServer.Api.Businesses.Business.Settings.BankRules.PaymentRules;
using ManagerServer.Api.Businesses.Business.Settings.BankRules.ReceiptRules;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.BankRules
{
    internal sealed record BankRulesResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetBankRules : AuthorizedEndpoint<BankRulesResource>
    {
        public override BankRulesResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["paymentRules"] = new Link(new GetPaymentRuleBatch { Business = Business }.ToUrl());
            links["receiptRules"] = new Link(new GetReceiptRuleBatch { Business = Business }.ToUrl());

            return new BankRulesResource(links);
        }
    }
}

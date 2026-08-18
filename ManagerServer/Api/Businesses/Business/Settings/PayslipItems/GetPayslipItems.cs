using ManagerServer.Api.Businesses.Business.Settings.PayslipItems.PayslipContributionItems;
using ManagerServer.Api.Businesses.Business.Settings.PayslipItems.PayslipDeductionItems;
using ManagerServer.Api.Businesses.Business.Settings.PayslipItems.PayslipEarningsItems;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.PayslipItems
{
    internal sealed record PayslipItemsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetPayslipItems : AuthorizedEndpoint<PayslipItemsResource>
    {
        public override PayslipItemsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["payslipContributionItems"] = new Link(new GetPayslipContributionItemBatch { Business = Business }.ToUrl());
            links["payslipDeductionItems"] = new Link(new GetPayslipDeductionItemBatch { Business = Business }.ToUrl());
            links["payslipEarningsItems"] = new Link(new GetPayslipEarningsItemBatch { Business = Business }.ToUrl());

            return new PayslipItemsResource(links);
        }
    }
}

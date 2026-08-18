using ManagerServer.Api.Businesses.Business.Settings.CashFlowStatementGroups.FinancingActivities;
using ManagerServer.Api.Businesses.Business.Settings.CashFlowStatementGroups.InvestingActivities;
using ManagerServer.Api.Businesses.Business.Settings.CashFlowStatementGroups.OperatingActivities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.CashFlowStatementGroups
{
    internal sealed record CashFlowStatementGroupsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetCashFlowStatementGroups : AuthorizedEndpoint<CashFlowStatementGroupsResource>
    {
        public override CashFlowStatementGroupsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["financingActivities"] = new Link(new GetCashFlowStatementFinancingActivityGroupBatch { Business = Business }.ToUrl());
            links["investingActivities"] = new Link(new GetCashFlowStatementInvestingActivityGroupBatch { Business = Business }.ToUrl());
            links["operatingActivities"] = new Link(new GetCashFlowStatementOperatingActivityGroupBatch { Business = Business }.ToUrl());

            return new CashFlowStatementGroupsResource(links);
        }
    }
}

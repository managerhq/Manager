using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.AmortizationEntries;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.BankAndCashAccounts;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.CapitalAccounts;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.Customers;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.DepreciationEntries;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.Employees;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.FixedAssets;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.IntangibleAssets;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.InventoryItems;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.Investments;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.SpecialAccounts;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts.Suppliers;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.ControlAccounts
{
    internal sealed record ControlAccountsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetControlAccounts : AuthorizedEndpoint<ControlAccountsResource>
    {
        public override ControlAccountsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["amortizationEntries"] = new Link(new GetControlAccountForIntangibleAssetsAccumulatedAmortizationBatch { Business = Business }.ToUrl());
            links["bankAndCashAccounts"] = new Link(new GetControlAccountForBankAccountsBatch { Business = Business }.ToUrl());
            links["capitalAccounts"] = new Link(new GetControlAccountForCapitalAccountsBatch { Business = Business }.ToUrl());
            links["customers"] = new Link(new GetControlAccountForCustomersBatch { Business = Business }.ToUrl());
            links["depreciationEntries"] = new Link(new GetControlAccountForFixedAssetsAccumulatedDepreciationBatch { Business = Business }.ToUrl());
            links["employees"] = new Link(new GetControlAccountForEmployeesBatch { Business = Business }.ToUrl());
            links["fixedAssets"] = new Link(new GetControlAccountForFixedAssetsBatch { Business = Business }.ToUrl());
            links["intangibleAssets"] = new Link(new GetControlAccountForIntangibleAssetsBatch { Business = Business }.ToUrl());
            links["inventoryItems"] = new Link(new GetControlAccountForInventoryItemsBatch { Business = Business }.ToUrl());
            links["investments"] = new Link(new GetControlAccountForInvestmentsBatch { Business = Business }.ToUrl());
            links["specialAccounts"] = new Link(new GetControlAccountForSpecialAccountsBatch { Business = Business }.ToUrl());
            links["suppliers"] = new Link(new GetControlAccountForSuppliersBatch { Business = Business }.ToUrl());

            return new ControlAccountsResource(links);
        }
    }
}

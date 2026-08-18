using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.BalanceSheetAccounts;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.BankAndCashAccounts;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.CapitalAccounts;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.Employees;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.FixedAssets;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.IntangibleAssets;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.InventoryItems;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.Investments;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.PurchaseInvoices;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.SalesInvoices;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances.SpecialAccounts;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances
{
    internal sealed record StartingBalancesResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetStartingBalances : AuthorizedEndpoint<StartingBalancesResource>
    {
        public override StartingBalancesResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["balanceSheetAccounts"] = new Link(new GetBalanceSheetAccountStartingBalanceBatch { Business = Business }.ToUrl());
            links["bankAndCashAccounts"] = new Link(new GetBankOrCashAccountStartingBalanceBatch { Business = Business }.ToUrl());
            links["capitalAccounts"] = new Link(new GetCapitalAccountStartingBalanceBatch { Business = Business }.ToUrl());
            links["employees"] = new Link(new GetEmployeeStartingBalanceBatch { Business = Business }.ToUrl());
            links["fixedAssets"] = new Link(new GetFixedAssetStartingBalanceBatch { Business = Business }.ToUrl());
            links["intangibleAssets"] = new Link(new GetIntangibleAssetStartingBalanceBatch { Business = Business }.ToUrl());
            links["inventoryItems"] = new Link(new GetInventoryItemStartingBalanceBatch { Business = Business }.ToUrl());
            links["investments"] = new Link(new GetInvestmentStartingBalanceBatch { Business = Business }.ToUrl());
            links["purchaseInvoices"] = new Link(new GetPurchaseInvoiceStartingBalanceBatch { Business = Business }.ToUrl());
            links["salesInvoices"] = new Link(new GetSalesInvoiceStartingBalanceBatch { Business = Business }.ToUrl());
            links["specialAccounts"] = new Link(new GetSpecialAccountStartingBalanceBatch { Business = Business }.ToUrl());

            return new StartingBalancesResource(links);
        }
    }
}

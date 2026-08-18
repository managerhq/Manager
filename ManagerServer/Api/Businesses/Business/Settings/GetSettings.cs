using ManagerServer.Api.Businesses.Business.Settings.AccessTokens;
using ManagerServer.Api.Businesses.Business.Settings.BankRules;
using ManagerServer.Api.Businesses.Business.Settings.BillableExpenses;
using ManagerServer.Api.Businesses.Business.Settings.BusinessDetails;
using ManagerServer.Api.Businesses.Business.Settings.CapitalSubaccounts;
using ManagerServer.Api.Businesses.Business.Settings.CashFlowStatementGroups;
using ManagerServer.Api.Businesses.Business.Settings.ChartOfAccounts;
using ManagerServer.Api.Businesses.Business.Settings.ControlAccounts;
using ManagerServer.Api.Businesses.Business.Settings.Currencies;
using ManagerServer.Api.Businesses.Business.Settings.CustomButtons;
using ManagerServer.Api.Businesses.Business.Settings.CustomFields;
using ManagerServer.Api.Businesses.Business.Settings.CustomerPortals;
using ManagerServer.Api.Businesses.Business.Settings.DateAndNumberFormat;
using ManagerServer.Api.Businesses.Business.Settings.Divisions;
using ManagerServer.Api.Businesses.Business.Settings.EmailSettings;
using ManagerServer.Api.Businesses.Business.Settings.ExpenseClaimPayers;
using ManagerServer.Api.Businesses.Business.Settings.Extensions;
using ManagerServer.Api.Businesses.Business.Settings.Footers;
using ManagerServer.Api.Businesses.Business.Settings.Forecasts;
using ManagerServer.Api.Businesses.Business.Settings.InventoryKits;
using ManagerServer.Api.Businesses.Business.Settings.InventoryLocations;
using ManagerServer.Api.Businesses.Business.Settings.InventoryUnitCosts;
using ManagerServer.Api.Businesses.Business.Settings.InvestmentMarketPrices;
using ManagerServer.Api.Businesses.Business.Settings.LockDate;
using ManagerServer.Api.Businesses.Business.Settings.NonInventoryItems;
using ManagerServer.Api.Businesses.Business.Settings.PayslipItems;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions;
using ManagerServer.Api.Businesses.Business.Settings.StartingBalances;
using ManagerServer.Api.Businesses.Business.Settings.TaxCodes;
using ManagerServer.Api.Businesses.Business.Settings.Themes;
using ManagerServer.Api.Businesses.Business.Settings.UserPermissions;
using ManagerServer.Api.Businesses.Business.Settings.WithholdingTax;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings
{
    internal sealed record SettingsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetSettings : AuthorizedEndpoint<SettingsResource>
    {
        public override SettingsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["accessTokens"]           = new Link(new GetAccessTokenBatch              { Business = Business }.ToUrl());
            links["bankRules"]              = new Link(new GetBankRules                     { Business = Business }.ToUrl());
            links["billableExpenses"]       = new Link(new GetBillableExpenses              { Business = Business }.ToUrl());
            links["businessDetails"]        = new Link(new GetBusinessDetails               { Business = Business }.ToUrl());
            links["capitalSubaccounts"]     = new Link(new GetSubAccountBatch               { Business = Business }.ToUrl());
            links["cashFlowStatementGroups"] = new Link(new GetCashFlowStatementGroups      { Business = Business }.ToUrl());
            links["chartOfAccounts"]        = new Link(new GetChartOfAccounts               { Business = Business }.ToUrl());
            links["controlAccounts"]        = new Link(new GetControlAccounts               { Business = Business }.ToUrl());
            links["currencies"]             = new Link(new GetCurrencies                    { Business = Business }.ToUrl());
            links["customFields"]           = new Link(new GetCustomFields                  { Business = Business }.ToUrl());
            links["customerPortals"]        = new Link(new GetCustomerPortalBatch           { Business = Business }.ToUrl());
            links["dateAndNumberFormat"]    = new Link(new GetDateAndNumberFormat            { Business = Business }.ToUrl());
            links["divisions"]              = new Link(new GetDivisionBatch                 { Business = Business }.ToUrl());
            links["emailSettings"]          = new Link(new GetEmailSettings                 { Business = Business }.ToUrl());
            links["expenseClaimPayers"]     = new Link(new GetExpenseClaimsPayerBatch        { Business = Business }.ToUrl());
            links["customButtons"]          = new Link(new GetCustomButtonBatch             { Business = Business }.ToUrl());
            links["footers"]                = new Link(new GetFooters                       { Business = Business }.ToUrl());
            links["forecasts"]              = new Link(new GetForecastBatch                 { Business = Business }.ToUrl());
            links["inventoryKits"]          = new Link(new GetInventoryKitBatch             { Business = Business }.ToUrl());
            links["inventoryLocations"]     = new Link(new GetInventoryLocations            { Business = Business }.ToUrl());
            links["inventoryUnitCosts"]     = new Link(new GetInventoryUnitCostBatch        { Business = Business }.ToUrl());
            links["investmentMarketPrices"] = new Link(new GetInvestmentMarketPriceBatch    { Business = Business }.ToUrl());
            links["lockDate"]               = new Link(new GetLockDate                      { Business = Business }.ToUrl());
            links["nonInventoryItems"]      = new Link(new GetNonInventoryItemBatch         { Business = Business }.ToUrl());
            links["payslipItems"]           = new Link(new GetPayslipItems                  { Business = Business }.ToUrl());
            links["recurringTransactions"]  = new Link(new GetRecurringTransactions          { Business = Business }.ToUrl());
            links["startingBalances"]       = new Link(new GetStartingBalances              { Business = Business }.ToUrl());
            links["taxCodes"]               = new Link(new GetTaxCodeBatch                  { Business = Business }.ToUrl());
            links["themes"]                 = new Link(new GetCustomThemeBatch                    { Business = Business }.ToUrl());
            links["userPermissions"]        = new Link(new GetUserPermissionsBatch           { Business = Business }.ToUrl());
            links["withholdingTax"]         = new Link(new GetWithholdingTax                { Business = Business }.ToUrl());

            return new SettingsResource(links);
        }
    }
}

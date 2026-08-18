using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.ChartOfAccounts
{
    internal sealed record ChartOfAccountsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetChartOfAccounts : AuthorizedEndpoint<ChartOfAccountsResource>
    {
        public override ChartOfAccountsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["balanceSheetAccounts"]              = new Link(new GetBalanceSheetAccountBatch              { Business = Business }.ToUrl());
            links["balanceSheetGroups"]                = new Link(new GetBalanceSheetGroupBatch                { Business = Business }.ToUrl());
            links["profitAndLossStatementAccounts"]    = new Link(new GetProfitAndLossStatementAccountBatch    { Business = Business }.ToUrl());
            links["profitAndLossStatementGroups"]      = new Link(new GetProfitAndLossStatementGroupBatch      { Business = Business }.ToUrl());
            links["subtotals"]                         = new Link(new GetSubtotalBatch                         { Business = Business }.ToUrl());

            links["balanceSheetAccountsPayableAccount"]                         = new Link(new GetBalanceSheetAccountsPayableAccount                         { Business = Business }.ToUrl());
            links["balanceSheetAccountsReceivableAccount"]                      = new Link(new GetBalanceSheetAccountsReceivableAccount                      { Business = Business }.ToUrl());
            links["balanceSheetBillableExpensesAccount"]                        = new Link(new GetBalanceSheetBillableExpensesAccount                        { Business = Business }.ToUrl());
            links["balanceSheetBillableTimeAccount"]                            = new Link(new GetBalanceSheetBillableTimeAccount                            { Business = Business }.ToUrl());
            links["balanceSheetCapitalAccountsAccount"]                         = new Link(new GetBalanceSheetCapitalAccountsAccount                         { Business = Business }.ToUrl());
            links["balanceSheetCashAtBankAccount"]                              = new Link(new GetBalanceSheetCashAtBankAccount                              { Business = Business }.ToUrl());
            links["balanceSheetEmployeeClearingAccount"]                        = new Link(new GetBalanceSheetEmployeeClearingAccount                        { Business = Business }.ToUrl());
            links["balanceSheetExpenseClaimsAccount"]                           = new Link(new GetBalanceSheetExpenseClaimsAccount                           { Business = Business }.ToUrl());
            links["balanceSheetFixedAssetsAccumulatedDepreciationAccount"]      = new Link(new GetBalanceSheetFixedAssetsAccumulatedDepreciationAccount      { Business = Business }.ToUrl());
            links["balanceSheetFixedAssetsAtCostAccount"]                       = new Link(new GetBalanceSheetFixedAssetsAtCostAccount                       { Business = Business }.ToUrl());
            links["balanceSheetIntangibleAssetsAccumulatedAmortizationAccount"] = new Link(new GetBalanceSheetIntangibleAssetsAccumulatedAmortizationAccount { Business = Business }.ToUrl());
            links["balanceSheetIntangibleAssetsAtCostAccount"]                  = new Link(new GetBalanceSheetIntangibleAssetsAtCostAccount                  { Business = Business }.ToUrl());
            links["balanceSheetInterAccountTransfers"]                          = new Link(new GetBalanceSheetInterAccountTransfers                          { Business = Business }.ToUrl());
            links["balanceSheetInventoryOnHandAccount"]                         = new Link(new GetBalanceSheetInventoryOnHandAccount                         { Business = Business }.ToUrl());
            links["balanceSheetInvestmentsAccount"]                             = new Link(new GetBalanceSheetInvestmentsAccount                             { Business = Business }.ToUrl());
            links["balanceSheetNegativeInventoryClearing"]                      = new Link(new GetBalanceSheetNegativeInventoryClearing                      { Business = Business }.ToUrl());
            links["balanceSheetRetainedEarningsAccount"]                        = new Link(new GetBalanceSheetRetainedEarningsAccount                        { Business = Business }.ToUrl());
            links["balanceSheetSpecialAccountsAccount"]                         = new Link(new GetBalanceSheetSpecialAccountsAccount                         { Business = Business }.ToUrl());
            links["balanceSheetSuspenseAccount"]                                = new Link(new GetBalanceSheetSuspenseAccount                                { Business = Business }.ToUrl());
            links["balanceSheetTaxPayableAccount"]                              = new Link(new GetBalanceSheetTaxPayableAccount                              { Business = Business }.ToUrl());
            links["balanceSheetWithholdingTaxAccount"]                          = new Link(new GetBalanceSheetWithholdingTaxAccount                          { Business = Business }.ToUrl());
            links["balanceSheetWithholdingTaxPayableAccount"]                   = new Link(new GetBalanceSheetWithholdingTaxPayableAccount                   { Business = Business }.ToUrl());
            links["balanceSheetWithholdingTaxReceivableAccount"]                = new Link(new GetBalanceSheetWithholdingTaxReceivableAccount                { Business = Business }.ToUrl());
            links["equity"]                                                     = new Link(new GetEquity                                                     { Business = Business }.ToUrl());

            links["profitAndLossStatementAccountBillableExpensesCost"]                = new Link(new GetProfitAndLossStatementAccountBillableExpensesCost                { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountBillableExpensesInvoiced"]            = new Link(new GetProfitAndLossStatementAccountBillableExpensesInvoiced            { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountBillableTimeInvoiced"]                = new Link(new GetProfitAndLossStatementAccountBillableTimeInvoiced                { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountBillableTimeMovement"]                = new Link(new GetProfitAndLossStatementAccountBillableTimeMovement                { Business = Business }.ToUrl());
            links["profitAndLossStatementCapitalGainsOnInvestments"]                  = new Link(new GetProfitAndLossStatementCapitalGainsOnInvestments                  { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountCurrencyGainsLosses"]                 = new Link(new GetProfitAndLossStatementAccountCurrencyGainsLosses                 { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountFixedAssetDepreciation"]              = new Link(new GetProfitAndLossStatementAccountFixedAssetDepreciation              { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountFixedAssetLossOnDisposal"]            = new Link(new GetProfitAndLossStatementAccountFixedAssetLossOnDisposal            { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountIntangibleAssetsAmortization"]        = new Link(new GetProfitAndLossStatementAccountIntangibleAssetsAmortization        { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal"] = new Link(new GetProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountInventoryPurchases"]                  = new Link(new GetProfitAndLossStatementAccountInventoryPurchases                  { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountInventorySales"]                      = new Link(new GetProfitAndLossStatementAccountInventorySales                      { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountInventoryWriteOffs"]                  = new Link(new GetProfitAndLossStatementAccountInventoryWriteOffs                  { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountLatePaymentFees"]                     = new Link(new GetProfitAndLossStatementAccountLatePaymentFees                     { Business = Business }.ToUrl());
            links["profitAndLossStatementAccountRoundingExpense"]                     = new Link(new GetProfitAndLossStatementAccountRoundingExpense                     { Business = Business }.ToUrl());
            links["profitAndLossStatementTotal"]                                      = new Link(new GetProfitAndLossStatementTotal                                      { Business = Business }.ToUrl());

            return new ChartOfAccountsResource(links);
        }
    }
}

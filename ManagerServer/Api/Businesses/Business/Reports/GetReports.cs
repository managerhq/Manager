using ManagerServer.Api.Businesses.Business.Reports.AgedPayables;
using ManagerServer.Api.Businesses.Business.Reports.AgedReceivables;
using ManagerServer.Api.Businesses.Business.Reports.AmortizationCalculationWorksheet;
using ManagerServer.Api.Businesses.Business.Reports.BalanceSheet;
using ManagerServer.Api.Businesses.Business.Reports.BalanceSheetByGroup;
using ManagerServer.Api.Businesses.Business.Reports.BankAccountSummary;
using ManagerServer.Api.Businesses.Business.Reports.BillableTimeSummary;
using ManagerServer.Api.Businesses.Business.Reports.CapitalAccountsSummary;
using ManagerServer.Api.Businesses.Business.Reports.CashFlowStatement;
using ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsTransactions;
using ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices;
using ManagerServer.Api.Businesses.Business.Reports.CustomerSummary;
using ManagerServer.Api.Businesses.Business.Reports.DepreciationCalculationWorksheet;
using ManagerServer.Api.Businesses.Business.Reports.DivisionExceptionReport;
using ManagerServer.Api.Businesses.Business.Reports.EmployeeSummary;
using ManagerServer.Api.Businesses.Business.Reports.ExpenseClaimsSummary;
using ManagerServer.Api.Businesses.Business.Reports.FixedAssetSummary;
using ManagerServer.Api.Businesses.Business.Reports.ForecastProfitAndLossStatement;
using ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerSummary;
using ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerTransactions;
using ManagerServer.Api.Businesses.Business.Reports.IntangibleAssetSummary;
using ManagerServer.Api.Businesses.Business.Reports.InventoryCostingCalculationWorksheet;
using ManagerServer.Api.Businesses.Business.Reports.InventoryPriceList;
using ManagerServer.Api.Businesses.Business.Reports.InventoryProfitMargin;
using ManagerServer.Api.Businesses.Business.Reports.InventoryQuantityByLocation;
using ManagerServer.Api.Businesses.Business.Reports.InventoryValueSummary;
using ManagerServer.Api.Businesses.Business.Reports.PayslipSummary;
using ManagerServer.Api.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee;
using ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatement;
using ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget;
using ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementByGroup;
using ManagerServer.Api.Businesses.Business.Reports.RealizedCurrencyGainsLosses;
using ManagerServer.Api.Businesses.Business.Reports.RealizedInvestmentGainsSummary;
using ManagerServer.Api.Businesses.Business.Reports.ReceiptsAndPaymentsSummary;
using ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField;
using ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer;
using ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByItem;
using ManagerServer.Api.Businesses.Business.Reports.StatementOfChangesInEquity;
using ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsTransactions;
using ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices;
using ManagerServer.Api.Businesses.Business.Reports.SupplierSummary;
using ManagerServer.Api.Businesses.Business.Reports.TaxablePurchasesPerSupplier;
using ManagerServer.Api.Businesses.Business.Reports.TaxableSalesPerCustomer;
using ManagerServer.Api.Businesses.Business.Reports.TaxAudit;
using ManagerServer.Api.Businesses.Business.Reports.TaxReconciliation;
using ManagerServer.Api.Businesses.Business.Reports.TaxSummary;
using ManagerServer.Api.Businesses.Business.Reports.TaxTotals;
using ManagerServer.Api.Businesses.Business.Reports.TaxTransactions;
using ManagerServer.Api.Businesses.Business.Reports.TrialBalance;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Reports
{
    internal sealed record ReportsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetReports : AuthorizedEndpoint<ReportsResource>
    {
        public override ReportsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["agedPayables"]                          = new Link(new GetAgedPayablesBatch                          { Business = Business }.ToUrl());
            links["agedReceivables"]                       = new Link(new GetAgedReceivablesBatch                       { Business = Business }.ToUrl());
            links["amortizationCalculationWorksheet"]      = new Link(new GetAmortizationCalculationWorksheetBatch      { Business = Business }.ToUrl());
            links["balanceSheet"]                          = new Link(new GetBalanceSheetBatch                          { Business = Business }.ToUrl());
            links["balanceSheetByGroup"]                   = new Link(new GetBalanceSheetByGroupBatch                   { Business = Business }.ToUrl());
            links["bankAccountSummary"]                    = new Link(new GetBankAccountSummaryBatch                    { Business = Business }.ToUrl());
            links["billableTimeSummary"]                   = new Link(new GetBillableTimeSummaryBatch                   { Business = Business }.ToUrl());
            links["capitalAccountsSummary"]                = new Link(new GetCapitalAccountsSummaryBatch                { Business = Business }.ToUrl());
            links["cashFlowStatement"]                     = new Link(new GetCashFlowStatementBatch                     { Business = Business }.ToUrl());
            links["customerStatementsTransactions"]        = new Link(new GetCustomerStatementsTransactionsBatch        { Business = Business }.ToUrl());
            links["customerStatementsUnpaidInvoices"]      = new Link(new GetCustomerStatementsUnpaidInvoicesBatch      { Business = Business }.ToUrl());
            links["customerSummary"]                       = new Link(new GetCustomerSummaryBatch                       { Business = Business }.ToUrl());
            links["depreciationCalculationWorksheet"]      = new Link(new GetDepreciationCalculationWorksheetBatch      { Business = Business }.ToUrl());
            links["divisionExceptionReport"]               = new Link(new GetDivisionExceptionReportBatch               { Business = Business }.ToUrl());
            links["employeeSummary"]                       = new Link(new GetEmployeeSummaryBatch                       { Business = Business }.ToUrl());
            links["expenseClaimsSummary"]                  = new Link(new GetExpenseClaimsSummaryBatch                  { Business = Business }.ToUrl());
            links["fixedAssetSummary"]                     = new Link(new GetFixedAssetSummaryBatch                     { Business = Business }.ToUrl());
            links["forecastProfitAndLossStatement"]        = new Link(new GetForecastProfitAndLossStatementBatch        { Business = Business }.ToUrl());
            links["generalLedgerSummary"]                  = new Link(new GetGeneralLedgerSummaryBatch                  { Business = Business }.ToUrl());
            links["generalLedgerTransactions"]             = new Link(new GetGeneralLedgerTransactionsBatch             { Business = Business }.ToUrl());
            links["intangibleAssetSummary"]                = new Link(new GetIntangibleAssetSummaryBatch                { Business = Business }.ToUrl());
            links["inventoryCostingCalculationWorksheet"]  = new Link(new GetInventoryCostingCalculationWorksheetBatch  { Business = Business }.ToUrl());
            links["inventoryPriceList"]                    = new Link(new GetInventoryPriceListBatch                    { Business = Business }.ToUrl());
            links["inventoryProfitMargin"]                 = new Link(new GetInventoryProfitMarginBatch                 { Business = Business }.ToUrl());
            links["inventoryQuantityByLocation"]           = new Link(new GetInventoryQuantityByLocationBatch           { Business = Business }.ToUrl());
            links["inventoryValueSummary"]                 = new Link(new GetInventoryValueSummaryBatch                 { Business = Business }.ToUrl());
            links["payslipSummary"]                        = new Link(new GetPayslipSummaryBatch                        { Business = Business }.ToUrl());
            links["payslipTotalsPerItemAndEmployee"]       = new Link(new GetPayslipTotalsPerItemAndEmployeeBatch       { Business = Business }.ToUrl());
            links["profitAndLossStatement"]                = new Link(new GetProfitAndLossStatementBatch                { Business = Business }.ToUrl());
            links["profitAndLossStatementActualVsBudget"]  = new Link(new GetProfitAndLossStatementActualVsBudgetBatch  { Business = Business }.ToUrl());
            links["profitAndLossStatementByGroup"]         = new Link(new GetProfitAndLossStatementByGroupBatch         { Business = Business }.ToUrl());
            links["realizedCurrencyGainsLosses"]           = new Link(new GetRealizedCurrencyGainsLossesBatch           { Business = Business }.ToUrl());
            links["realizedInvestmentGainsSummary"]        = new Link(new GetRealizedInvestmentGainsSummaryBatch        { Business = Business }.ToUrl());
            links["receiptsAndPaymentsSummary"]            = new Link(new GetReceiptsAndPaymentsSummaryBatch            { Business = Business }.ToUrl());
            links["salesInvoiceTotalsByCustomField"]       = new Link(new GetSalesInvoiceTotalsByCustomFieldBatch       { Business = Business }.ToUrl());
            links["salesInvoiceTotalsByCustomer"]          = new Link(new GetSalesInvoiceTotalsByCustomerBatch          { Business = Business }.ToUrl());
            links["salesInvoiceTotalsByItem"]              = new Link(new GetSalesInvoiceTotalsByItemBatch              { Business = Business }.ToUrl());
            links["statementOfChangesInEquity"]            = new Link(new GetStatementOfChangesInEquityBatch            { Business = Business }.ToUrl());
            links["supplierStatementsTransactions"]        = new Link(new GetSupplierStatementsTransactionsBatch        { Business = Business }.ToUrl());
            links["supplierStatementsUnpaidInvoices"]      = new Link(new GetSupplierStatementsUnpaidInvoicesBatch      { Business = Business }.ToUrl());
            links["supplierSummary"]                       = new Link(new GetSupplierSummaryBatch                       { Business = Business }.ToUrl());
            links["taxAudit"]                              = new Link(new GetTaxAuditBatch                              { Business = Business }.ToUrl());
            links["taxReconciliation"]                     = new Link(new GetTaxReconciliationBatch                     { Business = Business }.ToUrl());
            links["taxSummary"]                            = new Link(new GetTaxSummaryBatch                            { Business = Business }.ToUrl());
            links["taxTotals"]                             = new Link(new GetTaxTotalsBatch                             { Business = Business }.ToUrl());
            links["taxTransactions"]                       = new Link(new GetTaxTransactionsBatch                       { Business = Business }.ToUrl());
            links["taxablePurchasesPerSupplier"]           = new Link(new GetTaxablePurchasesPerSupplierBatch           { Business = Business }.ToUrl());
            links["taxableSalesPerCustomer"]               = new Link(new GetTaxableSalesPerCustomerBatch               { Business = Business }.ToUrl());
            links["trialBalance"]                          = new Link(new GetTrialBalanceBatch                          { Business = Business }.ToUrl());

            return new ReportsResource(links);
        }
    }
}

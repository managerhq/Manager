using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports
{
    [ProtoContract]
    [Title(nameof(Strings.Reports))]
    [Guide("The `Reports` tab provides comprehensive financial and operational reporting capabilities for your business. Access a wide range of standard reports including financial statements, tax reports, customer and supplier summaries, inventory analysis, and more.")]
    [Guide("Reports are organized into categories based on their function, making it easy to find the information you need. Each report can be customized with date ranges, filters, and other parameters to meet your specific requirements.")]
    [Header("Standard Reports")]
    [Guide("The system includes dozens of built-in reports covering all aspects of your business operations:")]
    [Guide("Financial statements such as `Profit and Loss Statement`, `Balance Sheet`, `Cash Flow Statement`, and `Statement of Changes in Equity` provide a complete picture of your financial position and performance.")]
    [Guide("General ledger reports including `Trial Balance`, `General Ledger Summary`, and `General Ledger Transactions` offer detailed insights into your accounting records.")]
    [Guide("Specialized reports for taxes, customers, suppliers, inventory, fixed assets, payroll, and more help you manage specific areas of your business effectively.")]
    [TabScreenshot("fa-print", nameof(Strings.Reports))]
    [Header("Custom Reports")]
    [Guide("Beyond the standard reports, you can create custom reports using `Advanced Queries` to extract and analyze data in ways that are unique to your business needs.")]
    [Guide("Custom reports provide ultimate flexibility, allowing you to combine data from multiple sources, apply complex filters, perform calculations, and format results exactly as required.")]
    [Namespace(typeof(Reports))]
    [LinkGuide("Learn more about creating custom reports:", typeof(NakedObjectsWithAdvancedQueries))]
    internal sealed class Reports : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var referrer = this.ToUrl();

            var reports = this.GetReports().GetAll();

            using (Div(@class: "flex flex-col lg:gap-4"))
            {
                using (Div(@class: "lg:columns-2 2xl:columns-3"))
                {
                    foreach (var reportGroup in reports.Where(x => x.Visible).GroupBy(x => x.Category))
                    {
                        using (Div(@class: "break-inside-avoid-column lg:pb-8"))
                        {
                            using (Div(@class: "card"))
                            {
                                using (Div(@class: "card-header")) using (Div(@class: "card-title")) Write(reportGroup.Key);

                                foreach (var report in reportGroup)
                                {
                                    if (report.HttpHandler is BusinessTemplate businessTemplate) businessTemplate.Referrer = referrer;
                                    using (A(href: report.HttpHandler.ToUrl(), @class: "card-body"))
                                    {
                                        Write(report.DisplayName);
                                    }
                                }
                            }
                        }
                    }

                    if (GetCurrentUserPermissions(Business).FullAccess)
                    {
                        var customButtons = GetCustomButtons();
                        if (customButtons.Length > 0)
                        {
                            using (Div(@class: "break-inside-avoid-column lg:pb-8"))
                            {
                                using (Div(@class: "card"))
                                {
                                    using (Div(@class: "card-header")) using (Div(@class: "card-title")) Write(Strings.CustomButtons);

                                    foreach (var e in customButtons)
                                    {
                                        EmitCustomButton(e, "card-body text-start cursor-pointer text-[var(--primary-foreground)]/75 hover:text-[var(--primary-foreground)]");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void EmitCustomButtons()
        {
            return;
        }

        protected override Task InnerPost()
        {
            Response.Redirect(this.ToUrl());
            return Task.CompletedTask;
        }

        public Collection GetReports()
        {
            var objects = ApplicationData.Businesses.Get(Business);
            var tabs = this.GetTabs(false, Business);
            var taxCodes = objects.OfType<ManagerServer.Model.TaxCode>().Any();
            var inventoryLocations = objects.OfType<ManagerServer.Model.CustomInventoryLocation>().Any();
            var forecasts = objects.OfType<ManagerServer.Model.Forecast>().Any();

            var priceList = objects.OfType<ManagerServer.Model.InventoryItem>().Any(x => x.DefaultSalesUnitPrice != 0m);

            var collection = new Collection();

            collection.ProfitAndLossStatement = new Item() { Column = 1, Category = Strings.FinancialStatements, DisplayName = Strings.ProfitAndLossStatement, HttpHandler = new HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement.ProfitAndLossStatementList() { Business = Business }, Visible = true };
            collection.ProfitAndLossStatementByGroup = new Item() { Column = 1, Category = Strings.FinancialStatements, DisplayName = Strings.ProfitAndLossStatementByGroup, HttpHandler = new HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementByGroup.ProfitAndLossStatementByGroupList() { Business = Business }, Visible = true };            
            collection.BalanceSheet = new Item() { Column = 1, Category = Strings.FinancialStatements, DisplayName = Strings.BalanceSheet, HttpHandler = new HttpHandlers.Businesses.Business.Reports.BalanceSheet.BalanceSheetList() { Business = Business }, Visible = true };
            collection.BalanceSheetByGroup = new Item() { Column = 1, Category = Strings.FinancialStatements, DisplayName = Strings.BalanceSheetByGroup, HttpHandler = new HttpHandlers.Businesses.Business.Reports.BalanceSheetByGroup.BalanceSheetByGroupList() { Business = Business }, Visible = true };
            collection.CashFlowStatement = new Item() { Column = 1, Category = Strings.FinancialStatements, DisplayName = Strings.CashFlowStatement, HttpHandler = new HttpHandlers.Businesses.Business.Reports.CashFlowStatement.CashFlowStatementList() { Business = Business }, Visible = true };
            collection.StatementOfChangesInEquity = new Item() { Column = 1, Category = Strings.FinancialStatements, DisplayName = Strings.StatementOfChangesInEquity, HttpHandler = new HttpHandlers.Businesses.Business.Reports.StatementOfChangesInEquity.StatementOfChangesInEquityList() { Business = Business }, Visible = true };

            collection.TrialBalance = new Item() { Column = 1, Category = Strings.General_ledger, DisplayName = Strings.TrialBalance, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TrialBalance.TrialBalanceList() { Business = Business }, Visible = true };
            collection.GeneralLedgerSummary = new Item() { Column = 1, Category = Strings.General_ledger, DisplayName = Strings.GeneralLedgerSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryList() { Business = Business }, Visible = true };
            collection.GeneralLedgerTransactions = new Item() { Column = 1, Category = Strings.General_ledger, DisplayName = Strings.GeneralLedgerTransactions, HttpHandler = new HttpHandlers.Businesses.Business.Reports.GeneralLedgerTransactions.GeneralLedgerTransactionsList() { Business = Business }, Visible = true };

            collection.ProfitAndLossStatementActualVsBudget = new Item() { Column = 1, Category = Strings.Forecasts, DisplayName = Strings.ProfitAndLossStatementActualVsBudget, HttpHandler = new HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget.ProfitAndLossStatementActualVsBudgetList() { Business = Business }, Visible = true };
            collection.ForecastProfitAndLossStatement = new Item() { Column = 1, Category = Strings.Forecasts, DisplayName = Strings.ForecastProfitAndLossStatement, HttpHandler = new HttpHandlers.Businesses.Business.Reports.ForecastProfitAndLossStatement.ForecastProfitAndLossStatementList() { Business = Business }, Visible = forecasts };

            collection.ReceiptsAndPaymentsSummary = new Item() { Column = 1, Category = Strings.CashAndCashEquivalents, DisplayName = Strings.ReceiptsAndPaymentsSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary.ReceiptsAndPaymentsSummaryList() { Business = Business }, Visible = tabs.BankAndCashAccounts.Visible };
            collection.BankAccountSummary = new Item() { Column = 1, Category = Strings.CashAndCashEquivalents, DisplayName = Strings.BankAccountSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.BankAccountSummary.BankAccountSummaryList() { Business = Business }, Visible = tabs.BankAndCashAccounts.Visible };

            collection.TaxAudit = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxAudit, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxAudit.TaxAuditList() { Business = Business }, Visible = taxCodes };
            collection.TaxSummary = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxSummary.TaxSummaryList() { Business = Business }, Visible = taxCodes, ReportType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.TaxSummary)) };
            collection.TaxTotals = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxTotals, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxTotals.TaxTotalsList() { Business = Business }, Visible = taxCodes, ReportType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.TaxTotals)) };
            collection.TaxReconciliation = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxReconciliation, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxReconciliation.TaxReconciliationList() { Business = Business }, Visible = taxCodes };
            collection.TaxTransactions = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxTransactions, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxTransactions.TaxTransactionsList() { Business = Business }, Visible = taxCodes };
            collection.TaxableSalesPerCustomer = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxableSalesPerCustomer, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxableSalesPerCustomer.TaxableSalesPerCustomerList() { Business = Business }, Visible = taxCodes && tabs.Customers.Visible };
            collection.TaxablePurchasesPerSupplier = new Item() { Column = 1, Category = Strings.TaxCodes, DisplayName = Strings.TaxablePurchasesPerSupplier, HttpHandler = new HttpHandlers.Businesses.Business.Reports.TaxablePurchasesPerSupplier.TaxablePurchasesPerSupplierList() { Business = Business }, Visible = taxCodes && tabs.Suppliers.Visible, ReportType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.TaxablePurchasesPerSupplier)) };

            collection.CustomerSummary = new Item() { Column = 2, Category = Strings.Customers, DisplayName = Strings.CustomerSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.CustomerSummary.CustomerSummaryList() { Business = Business }, Visible = tabs.Customers.Visible };
            collection.AgedReceivables = new Item() { Column = 2, Category = Strings.Customers, DisplayName = Strings.AgedReceivables, HttpHandler = new HttpHandlers.Businesses.Business.Reports.AgedReceivables.AgedReceivablesList() { Business = Business }, Visible = tabs.SalesInvoices.Visible || tabs.Customers.Visible };
            collection.CustomerStatementsUnpaidInvoices = new Item() { Column = 2, Category = Strings.Customers, DisplayName = Strings.CustomerStatementsUnpaidInvoices, HttpHandler = new HttpHandlers.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices.CustomerStatementsUnpaidInvoicesList() { Business = Business }, Visible = tabs.SalesInvoices.Visible || tabs.Customers.Visible };
            collection.CustomerStatementsTransactions = new Item() { Column = 2, Category = Strings.Customers, DisplayName = Strings.CustomerStatementsTransactions, HttpHandler = new HttpHandlers.Businesses.Business.Reports.CustomerStatementsTransactions.CustomerStatementsTransactionsList() { Business = Business }, Visible = tabs.SalesInvoices.Visible || tabs.Customers.Visible };

            collection.SalesInvoiceTotalsByCustomer = new Item() { Column = 2, Category = Strings.SalesInvoices, DisplayName = Strings.SalesInvoiceTotalsByCustomer, HttpHandler = new HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer.SalesInvoiceTotalsByCustomerList() { Business = Business }, Visible = tabs.SalesInvoices.Visible };
            collection.SalesInvoiceTotalsByItem = new Item() { Column = 2, Category = Strings.SalesInvoices, DisplayName = Strings.SalesInvoiceTotalsByItem, HttpHandler = new HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByItem.SalesInvoiceTotalsByItemList() { Business = Business }, Visible = tabs.SalesInvoices.Visible };
            collection.SalesInvoiceTotalsByCustomField = new Item() { Column = 2, Category = Strings.SalesInvoices, DisplayName = Strings.SalesInvoiceTotalsByCustomField, HttpHandler = new HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField.SalesInvoiceTotalsByCustomFieldList() { Business = Business }, Visible = tabs.SalesInvoices.Visible };

            collection.AgedPayables = new Item() { Column = 2, Category = Strings.Suppliers, DisplayName = Strings.AgedPayables, HttpHandler = new HttpHandlers.Businesses.Business.Reports.AgedPayables.AgedPayablesList() { Business = Business }, Visible = tabs.PurchaseInvoices.Visible || tabs.Suppliers.Visible };
            collection.SupplierSummary = new Item() { Column = 2, Category = Strings.Suppliers, DisplayName = Strings.SupplierSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.SupplierSummary.SupplierSummaryList() { Business = Business }, Visible = tabs.Suppliers.Visible };
            collection.SupplierStatementsUnpaidInvoices = new Item() { Column = 2, Category = Strings.Suppliers, DisplayName = Strings.SupplierStatementsUnpaidInvoices, HttpHandler = new HttpHandlers.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices.SupplierStatementsUnpaidInvoicesList() { Business = Business }, Visible = tabs.PurchaseInvoices.Visible || tabs.Suppliers.Visible };
            collection.SupplierStatementsTransactions = new Item() { Column = 2, Category = Strings.Suppliers, DisplayName = Strings.SupplierStatementsTransactions, HttpHandler = new HttpHandlers.Businesses.Business.Reports.SupplierStatementsTransactions.SupplierStatementsTransactionsList() { Business = Business }, Visible = tabs.PurchaseInvoices.Visible || tabs.Suppliers.Visible };
            collection.EmployeeStatementsTransactions = new Item() { Column = 2, Category = Strings.Employees, DisplayName = Strings.EmployeeStatementsTransactions, HttpHandler = new HttpHandlers.Businesses.Business.Reports.EmployeeStatementsTransactions.EmployeeStatementsTransactionsList() { Business = Business }, Visible = tabs.Employees.Visible };

            collection.InventoryValueSummary = new Item() { Column = 2, Category = Strings.InventoryItems, DisplayName = Strings.InventoryValueSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.InventoryValueSummary.InventoryValueSummaryList() { Business = Business }, Visible = tabs.InventoryItems.Visible };
            collection.InventoryQuantitySummary = new Item() { Column = 2, Category = Strings.InventoryItems, DisplayName = Strings.InventoryQuantitySummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.InventoryQuantitySummary.InventoryQuantitySummaryList() { Business = Business }, Visible = tabs.InventoryItems.Visible };
            collection.InventoryProfitMargin = new Item() { Column = 2, Category = Strings.InventoryItems, DisplayName = Strings.InventoryProfitMargin, HttpHandler = new HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin.InventoryProfitMarginList() { Business = Business }, Visible = tabs.InventoryItems.Visible };
            collection.InventoryQuantityByLocation = new Item() { Column = 2, Category = Strings.InventoryItems, DisplayName = Strings.InventoryQuantityByLocation, HttpHandler = new HttpHandlers.Businesses.Business.Reports.InventoryQuantityByLocation.InventoryQuantityByLocationList() { Business = Business }, Visible = inventoryLocations };
            collection.InventoryPriceList = new Item() { Column = 2, Category = Strings.InventoryItems, DisplayName = Strings.InventoryPriceList, HttpHandler = new HttpHandlers.Businesses.Business.Reports.InventoryPriceList.InventoryPriceListList() { Business = Business }, Visible = priceList };
            collection.InventoryCostingCalculationWorksheet = new Item() { Column = 2, Category = Strings.InventoryItems, DisplayName = Strings.InventoryCostingCalculationWorksheet, HttpHandler = new HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet.InventoryCostingCalculationWorksheetList() { Business = Business }, Visible = tabs.InventoryItems.Visible };

            collection.BillableTimeSummary = new Item() { Column = 2, Category = Strings.BillableTime, DisplayName = Strings.BillableTimeSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.BillableTimeSummary.BillableTimeSummaryList() { Business = Business }, Visible = tabs.BillableTime.Visible };
            collection.FixedAssetSummary = new Item() { Column = 2, Category = Strings.FixedAssets, DisplayName = Strings.FixedAssetSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.FixedAssetSummary.FixedAssetSummaryList() { Business = Business }, Visible = tabs.FixedAssets.Visible };
            collection.DepreciationCalculationWorksheet = new Item() { Column = 2, Category = Strings.FixedAssets, DisplayName = Strings.DepreciationCalculationWorksheet, HttpHandler = new HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet.DepreciationCalculationWorksheetList() { Business = Business }, Visible = tabs.FixedAssets.Visible };
            collection.IntangibleAssetSummary = new Item() { Column = 2, Category = Strings.IntangibleAssets, DisplayName = Strings.IntangibleAssetSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.IntangibleAssetSummary.IntangibleAssetSummaryList() { Business = Business }, Visible = tabs.IntangibleAssets.Visible };
            collection.AmortizationCalculationWorksheet = new Item() { Column = 2, Category = Strings.IntangibleAssets, DisplayName = Strings.AmortizationCalculationWorksheet, HttpHandler = new HttpHandlers.Businesses.Business.Reports.AmortizationCalculationWorksheet.AmortizationCalculationWorksheetList() { Business = Business }, Visible = tabs.IntangibleAssets.Visible };
            collection.ExpenseClaimsSummary = new Item() { Column = 2, Category = Strings.ExpenseClaims, DisplayName = Strings.ExpenseClaimsSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.ExpenseClaimsSummary.ExpenseClaimsSummaryList() { Business = Business }, Visible = tabs.ExpenseClaims.Visible };
            collection.PayslipSummary = new Item() { Column = 2, Category = Strings.Payslips, DisplayName = Strings.PayslipSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.PayslipSummary.PayslipSummaryList() { Business = Business }, Visible = tabs.Employees.Visible || tabs.Payslips.Visible };
            collection.PayslipTotals = new Item() { Column = 2, Category = Strings.Payslips, DisplayName = Strings.PayslipTotalsPerItemAndEmployee, HttpHandler = new HttpHandlers.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee.PayslipTotalsPerItemAndEmployeeList() { Business = Business }, Visible = tabs.Employees.Visible || tabs.Payslips.Visible, ReportType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PayslipTotalsPerItemAndEmployee)) };
            collection.CapitalAccountsSummary = new Item() { Column = 2, Category = Strings.CapitalAccounts, DisplayName = Strings.CapitalAccountsSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.CapitalAccountsSummary.CapitalAccountsSummaryList() { Business = Business }, Visible = tabs.CapitalAccounts.Visible };
            collection.RealizedInvestmentGainsSummary = new Item() { Column = 2, Category = Strings.Investments, DisplayName = Strings.RealizedInvestmentGainsLosses, HttpHandler = new HttpHandlers.Businesses.Business.Reports.Investments.RealizedInvestmentGainsSummary.RealizedInvestmentGainsSummaryList() { Business = Business }, Visible = tabs.Investments.Visible };
            collection.RealizedCurrencyGainsLosses = new Item() { Column = 2, Category = Strings.Currencies, DisplayName = Strings.RealizedCurrencyGainsAndLosses, HttpHandler = new HttpHandlers.Businesses.Business.Reports.Currencies.RealizedCurrencyGainsLosses.RealizedCurrencyGainsLossesList() { Business = Business }, Visible = objects.OfType<ForeignCurrency>().Any() };
            collection.DivisionExceptionReport = new Item() { Column = 2, Category = Strings.Divisions, DisplayName = Strings.DivisionExceptionReport, HttpHandler = new HttpHandlers.Businesses.Business.Reports.DivisionExceptionReport.DivisionExceptionReportList() { Business = Business }, Visible = objects.OfType<ManagerServer.Model.Division>().Any() };

            collection.EmployeeSummary = new Item() { Column = 2, Category = Strings.Employees, DisplayName = Strings.EmployeeSummary, HttpHandler = new HttpHandlers.Businesses.Business.Reports.EmployeeSummary.EmployeeSummaryList() { Business = Business }, Visible = tabs.Employees.Visible, ReportType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.EmployeeSummary)) };

            collection.CustomReports = new Item() { Column = 2, Category = Strings.CustomReports, DisplayName = Strings.CustomReports, HttpHandler = new HttpHandlers.Businesses.Business.Reports.CustomReports.CustomReports() { Business = Business }, Visible = true };

            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                foreach (var e in collection.GetAll())
                {
                    if (!userPermissions.CanView(e.HttpHandler.GetType().Namespace)) e.Visible = false;
                }
            }

            return collection;
        }

        public sealed class Collection
        {
            public Item ProfitAndLossStatement;
            public Item ProfitAndLossStatementByGroup;
            public Item BalanceSheet;
            public Item BalanceSheetByGroup;
            public Item StatementOfChangesInEquity;
            public Item TrialBalance;
            public Item GeneralLedgerSummary;
            public Item GeneralLedgerTransactions;
            public Item TaxAudit;
            public Item TaxSummary;
            public Item TaxTotals;
            public Item AgedReceivables;
            public Item AgedPayables;
            public Item InventoryValueSummary;
            public Item InventoryQuantitySummary;
            public Item InventoryProfitMargin;
            public Item InventoryQuantityByLocation;
            public Item InventoryPriceList;
            public Item FixedAssetSummary;
            public Item IntangibleAssetSummary;
            public Item CapitalAccountsSummary;
            public Item ReceiptsAndPaymentsSummary;
            public Item BankAccountSummary;
            public Item CustomerStatementsUnpaidInvoices;
            public Item CustomerStatementsTransactions;
            public Item TaxTransactions;
            public Item TaxReconciliation;
            public Item BillableTimeSummary;
            public Item PayslipSummary;
            public Item PayslipTotals;
            public Item ExpenseClaimsSummary;
            public Item SupplierStatementsUnpaidInvoices;
            public Item SupplierStatementsTransactions;
            public Item EmployeeStatementsTransactions;
            public Item DivisionExceptionReport;
            public Item CustomReports;
            public Item ProfitAndLossStatementActualVsBudget;
            public Item SalesInvoiceTotalsByCustomer;
            public Item SalesInvoiceTotalsByItem;
            public Item SalesInvoiceTotalsByCustomField;
            public Item EmployeeSummary;
            public Item TaxablePurchasesPerSupplier;
            public Item TaxableSalesPerCustomer;
            public Item CustomerSummary;
            public Item SupplierSummary;
            public Item DepreciationCalculationWorksheet;
            public Item AmortizationCalculationWorksheet;
            public Item CashFlowStatement;
            public Item ForecastProfitAndLossStatement;
            public Item RealizedInvestmentGainsSummary;
            public Item RealizedCurrencyGainsLosses;
            public Item InventoryCostingCalculationWorksheet;

            public Item[] GetAll()
            {
                return new Item[] {
                    ProfitAndLossStatement,
                    ProfitAndLossStatementByGroup,
                    ProfitAndLossStatementActualVsBudget,
                    BalanceSheet,
                    BalanceSheetByGroup,
                    CashFlowStatement,
                    StatementOfChangesInEquity,
                    ForecastProfitAndLossStatement,
                    ReceiptsAndPaymentsSummary,
                    BankAccountSummary,
                    TrialBalance,
                    GeneralLedgerSummary,
                    GeneralLedgerTransactions,
                    TaxAudit,
                    TaxSummary,
                    TaxTotals,
                    TaxReconciliation,
                    TaxTransactions,
                    AgedReceivables,
                    CustomerSummary,
                    CustomerStatementsUnpaidInvoices,
                    CustomerStatementsTransactions,
                    AgedPayables,
                    SupplierSummary,
                    SalesInvoiceTotalsByCustomer,
                    SalesInvoiceTotalsByItem,
                    SalesInvoiceTotalsByCustomField,
                    InventoryValueSummary,
                    InventoryQuantitySummary,
                    InventoryProfitMargin,
                    InventoryPriceList,
                    InventoryCostingCalculationWorksheet,
                    FixedAssetSummary,
                    DepreciationCalculationWorksheet,
                    IntangibleAssetSummary,
                    AmortizationCalculationWorksheet,
                    ExpenseClaimsSummary,
                    CapitalAccountsSummary,
                    BillableTimeSummary,
                    EmployeeSummary,
                    PayslipSummary,
                    PayslipTotals,
                    SupplierStatementsUnpaidInvoices,
                    SupplierStatementsTransactions,
                    EmployeeStatementsTransactions,
                    DivisionExceptionReport,
                    CustomReports,
                    InventoryQuantityByLocation,
                    TaxableSalesPerCustomer,
                    TaxablePurchasesPerSupplier,
                    RealizedInvestmentGainsSummary,
                    RealizedCurrencyGainsLosses,                    
                };
            }
        }

        public sealed class Item
        {
            public string Category;
            public int Column;
            public string DisplayName;
            public HttpHandler HttpHandler;
            public bool Visible;
            public Guid? ReportType;
        }
    }
}

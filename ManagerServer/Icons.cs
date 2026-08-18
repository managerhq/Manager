using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static class Icons
    {
        private static Dictionary<string, string> icons = new Dictionary<string, string>();

        static Icons()
        {
            icons.Add(nameof(HttpHandlers.Businesses.Business.Summary), "fa-table-columns");
            icons.Add(nameof(HttpHandlers.Businesses.Business.BankAndCashAccounts), "fa-coins");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Receipts), "fa-plus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Payments), "fa-minus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.InterAccountTransfers), "fa-money-bill-transfer");
            icons.Add(nameof(HttpHandlers.Businesses.Business.BankReconciliations), "fa-clipboard-check");
            icons.Add(nameof(HttpHandlers.Businesses.Business.ExpenseClaims), "fa-wallet");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Customers), "fa-people-line");
            icons.Add(nameof(HttpHandlers.Businesses.Business.SalesQuotes), "fa-drafting-compass");
            icons.Add(nameof(HttpHandlers.Businesses.Business.SalesOrders), "fa-shopping-basket");
            icons.Add(nameof(HttpHandlers.Businesses.Business.SalesInvoices), "fa-file-invoice");
            icons.Add(nameof(HttpHandlers.Businesses.Business.CreditNotes), "fa-cut");
            icons.Add(nameof(HttpHandlers.Businesses.Business.LatePaymentFees), "fa-bell");
            icons.Add(nameof(HttpHandlers.Businesses.Business.DeliveryNotes), "fa-truck");
            icons.Add(nameof(HttpHandlers.Businesses.Business.WithholdingTaxReceipts), "fa-receipt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Suppliers), "fa-city");
            icons.Add(nameof(HttpHandlers.Businesses.Business.PurchaseQuotes), "fa-drafting-compass");
            icons.Add(nameof(HttpHandlers.Businesses.Business.PurchaseOrders), "fa-shopping-cart");
            icons.Add(nameof(HttpHandlers.Businesses.Business.PurchaseInvoices), "fa-file-invoice");
            icons.Add(nameof(HttpHandlers.Businesses.Business.DebitNotes), "fa-cut");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Employees), "fa-id-card");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Payslips), "fa-money-check");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Projects), "fa-chart-bar");
            icons.Add(nameof(HttpHandlers.Businesses.Business.InventoryItems), "fa-boxes-stacked");
            icons.Add(nameof(HttpHandlers.Businesses.Business.InventoryTransfers), "fa-people-carry-box");
            icons.Add(nameof(HttpHandlers.Businesses.Business.InventoryWriteOffs), "fa-eraser");
            icons.Add(nameof(HttpHandlers.Businesses.Business.GoodsReceipts), "fa-truck-loading");
            icons.Add(nameof(HttpHandlers.Businesses.Business.ProductionOrders), "fa-industry");
            icons.Add(nameof(HttpHandlers.Businesses.Business.BillableTime), "fa-stopwatch");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Investments), "fa-chart-pie");
            icons.Add(nameof(HttpHandlers.Businesses.Business.FixedAssets), "fa-building");
            icons.Add(nameof(HttpHandlers.Businesses.Business.DepreciationEntries), "fa-arrow-down-wide-short");
            icons.Add(nameof(HttpHandlers.Businesses.Business.IntangibleAssets), "fa-wind");
            icons.Add(nameof(HttpHandlers.Businesses.Business.AmortizationEntries), "fa-sort-amount-down");
            icons.Add(nameof(HttpHandlers.Businesses.Business.CapitalAccounts), "fa-bars-progress");
            icons.Add(nameof(HttpHandlers.Businesses.Business.SpecialAccounts), "fa-cubes");
            icons.Add(nameof(HttpHandlers.Businesses.Business.JournalEntries), "fa-balance-scale");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Folders), "fa-folder-open");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Reports), "fa-print");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings), "fa-cog");

            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.AccessTokens), "fa-key");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Currencies), "fa-money-bill");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Currencies.BaseCurrency), "fa-money-bill-alt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Currencies.ForeignCurrencies), "fa-money-bills");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.BankRules), "fa-ruler-combined");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.BillableExpenses), "fa-briefcase");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Currencies.ExchangeRates), "fa-money-bill-transfer");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.BusinessDetails), "fa-circle-info");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CapitalSubaccounts), "fa-list");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups), "fa-list");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups.FinancingActivities), "fa-layer-group");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups.InvestingActivities), "fa-layer-group");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CashFlowStatementGroups.OperatingActivities), "fa-layer-group");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.ChartOfAccounts), "fa-sitemap");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.StartingBalances.BalanceSheetAccounts), "fa-sitemap");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.ControlAccounts), "fa-object-group");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomerPortals), "fa-street-view");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields), "fa-pen-to-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.DateAndNumberFormat), "fa-calendar-alt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Divisions), "fa-chart-pie");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings), "fa-at");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.SmtpServer), "fa-server");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates), "fa-stamp");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.InvestmentMarketPrices), "fa-chart-line");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.ExpenseClaimPayers), "fa-user-tie");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomButtons), "fa-computer-mouse");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ScriptExtensions), "fa-puzzle-piece");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Footers), "fa-file-signature");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.Forecasts), "fa-chart-line");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.InventoryKits), "fa-boxes-stacked");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.InventoryLocations), "fa-warehouse");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.InventoryLocations.CustomInventoryLocations), "fa-warehouse");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.InventoryLocations.DefaultInventoryLocation), "fa-warehouse");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures), "fa-scroll");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.LockDate), "fa-lock");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.NonInventoryItems), "fa-th");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts), "fa-calculator");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.PayslipItems), "fa-tasks-alt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipContributionItems), "fa-tasks-alt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipDeductionItems), "fa-tasks-alt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipEarningsItems), "fa-tasks-alt");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions), "fa-repeat");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.TaxCodes), "fa-percent");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomThemes), "fa-paint-roller");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.UserPermissions), "fa-user-lock");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ClassicCustomFields), "fa-font");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields.TextCustomFields), "fa-font");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields.NumberCustomFields), "fa-hashtag");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields.DateCustomFields), "fa-calendar-days");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields.CheckboxCustomFields), "fa-square-check");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields.MultipleValueCustomFields), "fa-tags");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.CustomFields.ImageCustomFields), "fa-images");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.WithholdingTax), "fa-percent");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.WebServices), "fa-globe");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.StartingBalances), "fa-wand-magic-sparkles");

            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.BankRules.ReceiptRules), "fa-plus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.BankRules.PaymentRules), "fa-minus-square");

            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringReceipts), "fa-plus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPayments), "fa-minus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringInterAccountTransfers), "fa-money-bill-transfer");
            //icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringExpenseClaims), "fa-wallet");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesQuotes), "fa-drafting-compass");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesOrders), "fa-shopping-basket");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesInvoices), "fa-file-invoice");
            //icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringDeliveryNotes), "fa-truck");
            //icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseQuotes), "fa-drafting-compass");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseOrders), "fa-shopping-cart");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseInvoices), "fa-file-invoice");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPayslips), "fa-money-check");
            //icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringGoodsReceipts), "fa-truck-loading");
            //icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringProductionOrders), "fa-conveyor-belt");
            //icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringBillableTime), "fa-stopwatch");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringJournalEntries), "fa-balance-scale");

            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.Receipt), "fa-plus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.Payment), "fa-minus-square");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.SalesQuote), "fa-drafting-compass");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.SalesOrder), "fa-shopping-basket");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.SalesInvoice), "fa-file-invoice");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.DeliveryNote), "fa-truck");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.PurchaseQuote), "fa-drafting-compass");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.PurchaseOrder), "fa-shopping-cart");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.PurchaseInvoice), "fa-file-invoice");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.Payslip), "fa-money-check");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.CustomerStatement), "fa-users-class");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.CreditNote), "fa-cut");
            icons.Add(nameof(HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.DebitNote), "fa-cut");
        }

        public static string GetIcon(string key)
        {
            if (icons.TryGetValue(key, out string value))
            {
                return value;
            }
            return "square-dashed";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Globalization;
using ManagerServer.HttpHandlers;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using System.Threading.Tasks;
using System.Text;
using ManagerServer.Model.Master;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.ChartOfAccounts))]
    [Guide("The *chart of accounts* is the foundation of your accounting system.")]
    [Guide("It organizes all accounts used to record transactions and generate financial reports.")]
    [Guide("Access the chart of accounts from the **Settings** tab.")]
    [SettingsItemScreenshot("fa-sitemap", nameof(Strings.ChartOfAccounts))]
    [Header("Overview")]
    [Guide("The chart of accounts is organized into two main sections:")]
    [Guide("The left section contains *balance sheet* accounts - assets, liabilities, and equity.")]
    [Guide("Balance sheet accounts track what you own, what you owe, and owner's equity.")]
    [Guide("Click **New Account** on the left to add custom balance sheet accounts.")]
    [DefaultButtonScreenshot(nameof(Strings.NewAccount))]
    [LinkGuide("Learn more:", typeof(BalanceSheetAccountForm))]
    [Header("Balance Sheet Groups")]
    [Guide("Organize balance sheet accounts into groups for better structure and reporting.")]
    [Guide("Common groups include Current Assets, Non-Current Assets, Current Liabilities, and Non-Current Liabilities.")]
    [Guide("Click **New Group** on the left to create custom account groups.")]
    [DefaultButtonScreenshot(nameof(Strings.NewGroup))]
    //[LinkGuide("For more information see:", typeof(BalanceSheetGroupForm))]
    [Header("Profit and Loss Accounts")]
    [Guide("The right section contains *profit and loss statement* accounts - income and expenses.")]
    [Guide("These accounts track revenues earned and expenses incurred during operations.")]
    [Guide("Click **New Account** on the right to add custom income or expense accounts.")]
    [DefaultButtonScreenshot(nameof(Strings.NewAccount))]
    //[LinkGuide("For more information see:", typeof(ProfitAndLossStatementAccountForm))]
    [Header("Income and Expense Groups")]
    [Guide("Group profit and loss accounts for organized reporting and analysis.")]
    [Guide("Common groups include Sales Revenue, Cost of Sales, Operating Expenses, and Other Income.")]
    [Guide("Click **New Group** on the right to create custom income and expense groups.")]
    [DefaultButtonScreenshot(nameof(Strings.NewGroup))]
    //[LinkGuide("For more information see:", typeof(ProfitAndLossStatementGroupForm))]
    [Header("Subtotals")]
    [Guide("Create subtotals to build a multi-step profit and loss statement.")]
    [Guide("Common subtotals include Gross Profit, Operating Profit, and Net Profit.")]
    [Guide("Click **New Total** to add subtotals that calculate intermediate results.")]
    [Guide("Subtotals make financial statements easier to read and analyze.")]
    [DefaultButtonScreenshot(nameof(Strings.NewTotal))]
    //[LinkGuide("For more information see:", typeof(ProfitAndLossStatementSubtotalForm))]
    [Header("Customizing Account Order")]
    [Guide("Customize the order of accounts and groups to match your reporting preferences.")]
    [Guide("Click the arrow icon next to any account or group to reorder items.")]
    [Guide("The order you set here determines how accounts appear on financial reports.")]
    //[LinkGuide("For more information see:", typeof(ReorderChartOfAccounts))]
    [Header("Balance Sheet Layouts")]
    [Guide("The main categories *Assets*, *Liabilities*, and *Equity* have fixed positions.")]
    [Guide("However, you can choose different balance sheet layouts when generating reports.")]
    [Guide("Report layouts can show these categories in different arrangements to suit your needs.")]
    //[LinkGuide("For more information see:", typeof(Reports.BalanceSheet.BalanceSheetForm))]
    [Header("System Accounts")]
    [Guide("Manager automatically creates system accounts based on the features you use.")]
    [Guide("These built-in accounts ensure proper integration between different modules.")]
    [Guide("You can rename system accounts to match your terminology, but cannot delete them while the related feature is active.")]
    [Header("Banking and Cash System Accounts")]
    [Guide("If you have added at least one bank or cash account under **Bank and Cash Accounts** tab, *Cash and Cash Equivalents* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetCashAtBankAccountForm))]
    [Guide("If you have added at least one bank or cash account under **Bank and Cash Accounts** tab, *Inter Account Transfers* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetInterAccountTransfersForm))]
    [Header("Receivables and Payables System Accounts")]
    [Guide("If you have added at least one customer under **Customers** tab, *Accounts Receivable* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetAccountsReceivableAccountForm))]
    [Guide("If you have added at least one supplier under **Suppliers** tab, *Accounts Payable* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetAccountsPayableAccountForm))]
    [Header("Billable Time and Expenses System Accounts")]
    [Guide("If you have created at least one billable time under **Billable Time** tab, *Billable Time* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetBillableTimeAccountForm))]
    [Guide("If you have enabled **Billable Expenses** feature, *Billable Expenses* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetBillableExpensesAccountForm))]
    [Header("Employee and Capital System Accounts")]
    [Guide("If you have added at least one capital account under **Capital Accounts** tab, *Capital Accounts* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetCapitalAccountsAccountForm))]
    [Guide("If you have added at least one employee under **Employees** tab, *Employee Clearing Account* will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetEmployeeClearingAccountForm))]
    [Guide("If you have added at least one expense claim payer under **Expense Claim Payers** section within **Settings** tab, *Expense Claims* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetExpenseClaimsAccountForm))]
    [Header("Fixed and Intangible Assets System Accounts")]
    [Guide("If you have added at least one fixed asset under **Fixed Assets** tab, *Fixed Assets at Cost* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetFixedAssetsAtCostAccountForm))]
    [Guide("If you have added at least one fixed asset under **Fixed Assets** tab, *Fixed Assets - Accumulated Depreciation* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetFixedAssetsAccumulatedDepreciationAccountForm))]
    [Guide("If you have added at least one intangible asset under **Intangible Assets** tab, *Intangible Assets at Cost* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetIntangibleAssetsAtCostAccountForm))]
    [Guide("If you have added at least one intangible asset under **Intangible Assets** tab, *Intangible Assets - Accumulated Amortization* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetIntangibleAssetsAccumulatedAmortizationAccountForm))]
    [Header("Inventory and Investment System Accounts")]
    [Guide("If you have added at least one inventory revaluation under **Inventory Revaluations** tab, *Inventory on Hand* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetInventoryOnHandAccountForm))]
    [Guide("If you have added at least one investment under **Investments** tab, *Investments at Cost* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetInvestmentsAccountForm))]
    [Guide("If you have added at least one special account under **Special Accounts** tab, *Special Accounts* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetSpecialAccountsAccountForm))]
    [Header("Tax and Retained Earnings System Accounts")]
    [Guide("If you have added at least one tax code under **Tax Codes** within **Settings** tab, *Tax Payable* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetTaxPayableAccountForm))]
    [Guide("If you have added at least one withholding tax receipt under **Withholding Tax Receipts** tab, *Withholding Tax* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetWithholdingTaxAccountForm))]
    [Guide("If you have enabled withholding tax for sales invoices under **Withholding Taxes** within **Settings** tab, *Withholding Tax Receivable* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetWithholdingTaxReceivableAccountForm))]
    [Guide("If you have enabled withholding tax for purchase invoices under **Withholding Taxes** within **Settings** tab, *Withholding Tax Payable* account will be added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetWithholdingTaxPayableAccountForm))]
    [Guide("*Retained Earnings* account is automatically added.")]
    [LinkGuide("Learn more:", typeof(BalanceSheetRetainedEarningsAccountForm))]
    [Header("Income Statement - Billable Time and Expenses")]
    [Guide("If you have enabled billable expenses under **Billable Expenses** within **Settings** tab, *Billable Expenses - Cost* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountBillableExpensesCostForm))]
    [Guide("If you have enabled billable expenses under **Billable Expenses** within **Settings** tab, *Billable Expenses - Invoiced* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountBillableExpensesInvoicedForm))]
    [Guide("If you have recorded at least one billable time under **Billable Time** tab, *Billable Time - Invoiced* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountBillableTimeInvoicedForm))]
    [Guide("If you have recorded at least one billable time under **Billable Time** tab, *Billable Time - Movement* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountBillableTimeMovementForm))]
    [Header("Income Statement - Gains and Losses")]
    [Guide("If you have recorded at least one investment market price under **Investment Market Prices** within **Settings** tab, *Investment Gains (Losses)* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountCapitalGainsOnInvestmentsForm))]
    [Guide("If you have created at least one foreign currency under **Currencies** then **Foreign Currencies** within **Settings** tab, *Currency Gains (Losses)* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountCurrencyGainsLossesForm))]
    [Header("Income Statement - Depreciation and Amortization")]
    [Guide("If you have created at least one depreciation entry under **Depreciation Entries** tab, *Fixed Assets - Depreciation* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountFixedAssetDepreciationForm))]
    [Guide("If you have marked at least one fixed asset as disposed under **Fixed Assets** tab, *Fixed Assets - Loss on Disposal* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountFixedAssetLossOnDisposalForm))]
    [Guide("If you have created at least one amortization entry under **Amortization Entries** tab, *Intangible Assets - Amortization* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountIntangibleAssetsAmortizationForm))]
    [Guide("If you have marked at least one intangible asset as disposed under **Intangible Assets** tab, *Intangible Assets - Gains (Losses) on Disposal* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposalForm))]
    [Header("Income Statement - Inventory and Other")]
    [Guide("If you have added at least one inventory item under **Inventory Items** tab, *Inventory - Sales* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountInventorySalesForm))]
    [Guide("If you have added at least one inventory item under **Inventory Items** tab, *Inventory - Cost* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountInventoryPurchasesForm))]
    [Guide("If you have created at least one late payment fee under **Late Payment Fees** tab, *Late Payment Fees* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountLatePaymentFeesForm))]
    [Guide("If you have created at least one sales invoice with rounding enabled under **Sales Invoices** tab, *Rounding Expense* account will be added.")]
    [LinkGuide("Learn more:", typeof(ProfitAndLossStatementAccountRoundingExpenseForm))]
    internal sealed class ChartOfAccounts : NakedObjectsWithJsonOutput
    {
        public object GetModel()
        {
            var model = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);

            var database = ApplicationData.Businesses.Get(Business);

            var balanceSheetGroups = new List<BalanceSheetAbstractGroup>();
            balanceSheetGroups.Add(database.Single<Assets>());
            balanceSheetGroups.Add(database.Single<Liabilities>());
            balanceSheetGroups.Add(database.Single<Equity>());
            balanceSheetGroups.AddRange(database.OfType<BalanceSheetGroup>());

            return new
            {
                groups = new
                {
                    balanceSheet2 = balanceSheetGroups.ToDictionary(x => x.Key, x => new
                    {
                        name = x.GetName(),
                        group = x is BalanceSheetGroup balanceSheetGroup ? balanceSheetGroup.Group : null
                    }),
                    profitAndLossStatement = database.OfType<ProfitAndLossStatementGroup>().ToDictionary(x => x.Key, x => new
                    {
                        name = x.Name,
                        type = x.Type,
                        position = x.Position,
                        group = x.Type == ManagerServer.Model.Enums.ProfitAndLossStatementGroupType.SubgroupOf ? x.Group : null
                    })
                },
                hierarchy = new
                {
                    balanceSheet = model.BalanceSheet.SelectMany(x => x.GetAllAccounts()).Select(x => new { key = x.Key, name = x.Name, groups = GetGroups(x) }).ToArray(),
                    profitAndLossStatement = model.ProfitAndLossStatement.SelectMany(x => x.GetAllAccounts()).Select(x => new { key = x.Key, name = x.Name, groups = GetGroups(x) }).ToArray()
                }
            };
        }

        private string[] GetGroups(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
        {
            var output = new List<string>();
            var current = account.Parent;
            while (current != null)
            {
                output.Add(current.Name);
                current = current.Parent;
            }
            output.Reverse();
            return output.ToArray();
        }

        private object GetHierarchy(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                return new { key = account.Key, name = account.Name };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                if (group.IsSubtotal)
                {
                    return new { runningTotal = group.Name };
                }
                else
                {
                    return group.Items.ToDictionary(x => x.Name, x => GetHierarchy(x));
                }
            }
            return null;
        }

        // This is essential for API2 to work. Some users rely on this.
        [Default] public string[] GetKey(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account[] account) => account.Select(x => x.Key.ToString()).ToArray();
        [Default] public string[] GetCode(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account[] account) => account.Select(x => x.Code).ToArray();
        [Default] public string[] GetName(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account[] account) => account.Select(x => x.Name).ToArray();

        protected override void InnerGet4(Context context)
        {
            var model = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);

            if (JsonOutput)
            {
                var accounts = new List<ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account>();
                accounts.AddRange(model.BalanceSheet.SelectMany(x => x.GetAllAccounts()));
                accounts.AddRange(model.ProfitAndLossStatement.SelectMany(x => x.GetAllAccounts()));

                context.Set<Array>(accounts.ToArray());

                base.InnerGet4(context);
                return;
            }

            var referrer = this.ToUrl();

            using (Div(@class: "flex flex-col lg:gap-4"))
            {
                /*
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-header flex gap-2 items-center"))
                    {
                        using (Div(@class: "card-title")) Write(Strings.ChartOfAccounts);
                        WriteHelp();
                    }
                }
                */

                using (Div(@class: "lg:columns-2 lg:gap-4"))
                {
                    using (Div(@class: "break-inside-avoid-column"))
                    {
                        using (Div(@class: "card"))
                        {
                            using (Div(@class: "card-header"))
                            {
                                using (Div(@class: "flex gap-4 items-center"))
                                {
                                    using (Div(@class: "card-title")) Write(Strings.BalanceSheet);
                                    using (A(href: new BalanceSheetGroupForm() { Business = Business, Referrer = referrer }.ToUrl(), @class: "btn")) Write(Strings.NewGroup);
                                    using (A(href: new BalanceSheetAccountForm() { Business = Business, Referrer = referrer }.ToUrl(), @class: "btn")) Write(Strings.NewAccount);
                                }
                            }
                            using (Table(@class: "card-table"))
                            {
                                using (THead())
                                {
                                    using (Tr())
                                    {
                                        using (Th(@class: "text-center w-1")) I(@class: "fas fa-edit", style: "font-size: 16px; opacity: 0.25");
                                        using (Th()) Write(Strings.Name);
                                        using (Th(@class: "w-1")) { }
                                    }
                                }

                                foreach (var e in model.BalanceSheet)
                                {
                                    if (e.Key == Guid.Empty && !e.Items.Any()) continue;
                                    printItem(e, referrer, 0, true);
                                }
                            }

                            using (Div(@class: "card-header")) { }
                        }
                    }

                    using (Div(@class: "break-inside-avoid-column"))
                    {
                        using (Div(@class: "card"))
                        {
                            using (Div(@class: "card-header"))
                            {
                                using (Div(@class: "flex flex gap-4 items-center"))
                                {
                                    using (Div(@class: "card-title")) Write(Strings.ProfitAndLossStatement);
                                    using (A(href: new ProfitAndLossStatementGroupForm() { Business = Business, Referrer = referrer }.ToUrl(), @class: "btn")) Write(Strings.NewGroup);
                                    using (A(href: new ProfitAndLossStatementAccountForm() { Business = Business, Referrer = referrer }.ToUrl(), @class: "btn")) Write(Strings.NewAccount);
                                    using (A(href: new ProfitAndLossStatementSubtotalForm() { Business = Business, Referrer = referrer }.ToUrl(), @class: "btn")) Write(Strings.NewTotal);
                                }
                            }
                            using (Table(@class: "card-table"))
                            {
                                using (THead())
                                {
                                    using (Tr())
                                    {
                                        using (Th(@class: "text-center w-1")) I(@class: "fas fa-edit", style: "font-size: 16px; opacity: 0.25");
                                        using (Th()) Write(Strings.Name);
                                        using (Th(@class: "w-1")) { }
                                    }
                                }

                                foreach (var e in model.ProfitAndLossStatement)
                                {
                                    if (e.Key == Guid.Empty && !e.Items.Any()) continue;
                                    printItem(e, referrer, 0, false);
                                }
                            }

                            using (Div(@class: "card-header")) { }
                        }
                    }
                }
            }
        }

        private void printItem(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, string referrer, int level, bool isBalanceSheetItem)
        {
            if (item.Key == ManagerServer.Model.Master.AccountKeys.Suspense) return;

            var isGroup = item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group;
            var isSubtotal = isGroup && ((ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group)item).IsSubtotal;

            using (Tr())
            {                
                using (Td(style: "width: 1px; text-align: center; white-space: nowrap"))
                {
                    if (isBalanceSheetItem)
                    {
                        if (isGroup)
                        {
                            if (item.Key == Guid.Empty || item.Key == ChartOfAccountGroups.Assets || item.Key == ChartOfAccountGroups.Liabilities)
                            {
                            }
                            else if (item.Key == ChartOfAccountGroups.Equity)
                            {
                                using (A(href: new BalanceSheetEquityGroupNameForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else
                            {
                                using (A(href: new BalanceSheetGroupForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                        }
                        else
                        {
                            var account = (ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account)item;
                            if (account.IsSystemAccount)
                            {
                                var type = ManagerServer.Model.Object.GetTypeByGuid(item.Key);
                                var genericType = typeof(NakedVueForm<>).MakeGenericType(type);
                                var formType = this.GetType().Assembly.GetTypes().SingleOrDefault(x => x.BaseType == genericType);
                                if (formType != null)
                                {
                                    var form = Activator.CreateInstance(formType) as ManagerServer.HttpHandlers.Businesses.Business.Form;
                                    form.Key = item.Key;
                                    form.Business = Business;
                                    form.Referrer = referrer;
                                    using (A(href: form.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                                }
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.BankAccounts)
                            {
                                using (A(href: new ControlAccounts.BankAndCashAccounts.ControlAccountForBankAccountsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.Customers)
                            {
                                using (A(href: new ControlAccounts.Customers.ControlAccountForCustomersForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.Suppliers)
                            {
                                using (A(href: new ControlAccounts.Suppliers.ControlAccountForSuppliersForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.InventoryItems)
                            {
                                using (A(href: new ControlAccounts.InventoryItems.ControlAccountForInventoryItemsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.Investments)
                            {
                                using (A(href: new ControlAccounts.Investments.ControlAccountForInvestmentsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.FixedAssets)
                            {
                                using (A(href: new ControlAccounts.FixedAssets.ControlAccountForFixedAssetsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.FixedAssetsAccumulatedDepreciation)
                            {
                                using (A(href: new ControlAccounts.DepreciationEntries.ControlAccountForFixedAssetsAccumulatedDepreciationForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.IntangibleAssets)
                            {
                                using (A(href: new ControlAccounts.IntangibleAssets.ControlAccountForIntangibleAssetsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.IntangibleAssetsAccumulatedAmortization)
                            {
                                using (A(href: new ControlAccounts.AmortizationEntries.ControlAccountForIntangibleAssetsAccumulatedAmortizationForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.CapitalAccounts)
                            {
                                using (A(href: new ControlAccounts.CapitalAccounts.ControlAccountForCapitalAccountsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.SpecialAccounts)
                            {
                                using (A(href: new ControlAccounts.SpecialAccounts.ControlAccountForSpecialAccountsForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (account.ControlAccountType == ManagerServer.Model.Enums.ControlAccountType.Employees)
                            {
                                using (A(href: new ControlAccounts.Employees.ControlAccountForEmployeesForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                            else if (!account.ControlAccountType.HasValue)
                            {
                                using (A(href: new BalanceSheetAccountForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                        }
                    }
                    else
                    {
                        if (isGroup)
                        {
                            var group = (ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group)item;
                            if (item.Key == Guid.Empty)
                            {
                            }
                            else if (group.IsSubtotal)
                            {
                                if (group.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.ProfitAndLossStatementTotal)))
                                {
                                    using (A(href: new ProfitAndLossStatementTotalForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                                }
                                else
                                {
                                    using (A(href: new ProfitAndLossStatementSubtotalForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                                }
                            }
                            else
                            {
                                using (A(href: new ProfitAndLossStatementGroupForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                        }
                        else
                        {
                            if (((ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account)item).IsSystemAccount)
                            {
                                var type = ManagerServer.Model.Object.GetTypeByGuid(item.Key);
                                var genericType = typeof(NakedVueForm<>).MakeGenericType(type);
                                var formType = this.GetType().Assembly.GetTypes().SingleOrDefault(x => x.BaseType == genericType);
                                if (formType != null)
                                {
                                    var form = Activator.CreateInstance(formType) as ManagerServer.HttpHandlers.Businesses.Business.Form;
                                    form.Key = item.Key;
                                    form.Business = Business;
                                    form.Referrer = referrer;
                                    using (A(href: form.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                                }
                            }
                            else
                            {
                                using (A(href: new ProfitAndLossStatementAccountForm() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                            }
                        }
                    }
                }
                using (Td())
                {
                    var padding = "padding-left: ";
                    if (ManagerServer.Globalization.Languages.IsRightToLeft()) padding = "padding-right: ";
                    using (Div(style: padding + (level * 20) + @"px"))
                    {
                        if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account)
                        {
                            var item2 = (ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account)item;
                            if (item2.IsSystemAccount)
                            {
                                if (item2.SystemName != item2.Name)
                                {
                                    using (Span(style: "text-decoration: line-through; color: #999; margin-right: 10px")) Write(item2.SystemName);
                                }
                            }
                        }

                        using (Span(style: item.Inactive ? "color: #ccc" : null))
                        {
                            using (Span(@class: (isGroup ? "font-semibold" : null)))
                            {
                                using (Span(style: (isSubtotal ? "border-bottom: 1px dotted #000; padding-bottom: 2px" : null)))
                                {
                                    if (isGroup)
                                    {
                                        var group = (ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group)item;
                                        if (group.IsExpenseGroup && level == 0)
                                        {
                                            Write(Strings.Less + ":&nbsp;");
                                        }
                                    }

                                    Write(item.Name);

                                    if (item.Key == ChartOfAccountGroups.Equity && item.Name != Strings.Equity)
                                    {
                                        using (Span(style: "text-decoration: line-through; color: #999; margin-left: 10px")) Write(Strings.Equity);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(item.Code)) using (Code(style: "margin-left : 5px; border: 1px solid #ccc; background-color: #fff; color: #333; padding: 3px 6px; border-radius: 10px")) Write(item.Code);
                    }
                }
                using (Td(style: "width: 1px; text-align: center; white-space: nowrap"))
                {
                    if (isBalanceSheetItem && level == 0)
                    {
                    }
                    else
                    {
                        using (A(href: new ReorderChartOfAccounts() { Business = Business, Key = item.Key, Referrer = referrer }.ToUrl())) I(@class: "fas fa-arrows-v", style: "font-size: 16px; opacity: 0.5");
                    }
                }
            }
            if (isGroup)
            {
                foreach (var e in ((ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group)item).Items)
                {
                    printItem(e, referrer, level + 1, isBalanceSheetItem);
                }
            }
        }
    }
}

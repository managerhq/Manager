using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipEarningsItems
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.PayslipEarningsItems))]
    [Guide("Payslip earnings items define the different types of payments that can be included on employee payslips.")]
    [Guide("Set up standard earnings categories such as regular wages, overtime pay, bonuses, commissions, allowances, and other compensation types.")]
    [Header("Overview")]
    [Guide("Each earnings item you create becomes available for selection when preparing payslips. This ensures consistency in how different types of earnings are recorded and reported.")]
    [Guide("Earnings items are linked to expense accounts in your chart of accounts, allowing automatic posting of wage costs to the correct accounts.")]
    [Header("Getting Started")]
    [Guide("To create a new earnings item, click the **New Payslip Item** button. You can create as many earnings items as needed to match your payroll structure.")]
    [Guide("Common earnings items include basic salary, overtime rates, performance bonuses, sales commissions, housing allowances, and travel allowances.")]
    [Columns]
    [NewButton(nameof(Strings.NewPayslipItem))]
    internal sealed class PayslipEarningsItems : PersistentObjectTable<ManagerServer.Model.PayslipEarningsItem>
    {
        [Guid("e7ce1d13-6543-41af-9a74-6887c2c05930")]
        [Guide("The name of the earnings item that will appear on payslips.")]
        [Guide("Examples include basic salary, overtime pay, bonuses, commissions, or allowances.")]
        [Guide("Choose descriptive names that employees will easily understand on their payslips.")]
        public string GetName(ManagerServer.Model.PayslipEarningsItem row) => row.Name;

        [Guid("9ec6d254-0004-4a34-8b5d-ba47e03b7cf4")]
        [Guide("The expense account where this earnings item will be posted.")]
        [Guide("This is typically a wages or salaries expense account in your *profit and loss statement*.")]
        [Guide("If no account is selected, earnings will be posted to the *suspense account*.")]
        public NamedObject GetExpenseAccount(ManagerServer.Model.PayslipEarningsItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ProfitAndLossStatementAccount>(row.ExpenseAccount) as NamedObject ?? ApplicationData.Businesses.Get(Business).Single<BalanceSheetSuspenseAccount>();

        [HideColumnIfAllEmpty]
        [Guid("c13fe7d9-2173-4052-b869-0fed4f086072")]
        [Guide("The reporting category for grouping earnings items in reports.")]
        [Guide("Use reporting categories to analyze payroll costs by type, department, or any other classification relevant to your business.")]
        public NamedObject GetReportingCategory(ManagerServer.Model.PayslipEarningsItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<PayslipEarningsItemReportingCategory>(row.ReportingCategory);
    }
}

using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipDeductionItems
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.PayslipDeductionItems))]
    [Guide("Payslip deduction items are used to define various types of deductions that can be applied to employee payslips. These represent amounts withheld from employees' gross pay.")]
    [Guide("Common examples include *income tax*, *social security contributions*, *health insurance premiums*, *retirement fund contributions*, and *loan repayments*.")]
    [Header("Setting Up Deduction Items")]
    [Guide("To create a new deduction item, click the **New Payslip Item** button. Each deduction item requires a name and can be linked to an account for proper accounting.")]
    [Guide("Once created, deduction items can be selected when preparing individual payslips. The same deduction item can be used across multiple employees and pay periods.")]
    [Header("Managing Your Deductions")]
    [Guide("This list shows all deduction items available in your business. You can edit existing items or create new ones as your payroll requirements change.")]
    [Guide("Deduction items can be made inactive when no longer needed. Inactive items won't appear in selection lists but remain available for historical records.")]
    [Columns]
    [NewButton(nameof(Strings.NewPayslipItem))]
    internal sealed class PayslipDeductionItems : PersistentObjectTable<ManagerServer.Model.PayslipDeductionItem>
    {
        [Guid("e11f2ce1-109b-46a0-aef7-697890366399")]
        [Guide("The descriptive name identifying this deduction item. This name will appear on employee payslips and in payroll reports.")]
        [Guide("Choose names that clearly indicate the purpose of the deduction, such as *Federal Income Tax*, *State Income Tax*, *Health Insurance Premium*, *401(k) Contribution*, or *Student Loan Repayment*.")]
        [Guide("Using standardized, consistent naming helps with payroll reporting and ensures employees easily understand their deductions.")]
        public string GetName(ManagerServer.Model.PayslipDeductionItem row) => row.Name;

        [Guid("fde75289-50e8-467b-b702-39c1e1aa3d59")]
        [Guide("The account where deducted amounts will be posted. This determines how the deduction is recorded in your accounting system.")]
        [Guide("Typically, deductions are posted to *liability accounts* (for amounts owed to third parties like tax authorities) or *expense accounts* (for employer-paid benefits).")]
        [Guide("If no account is selected, deductions will be posted to the *Suspense* account, which should be cleared by allocating to proper accounts.")]
        public NamedObject GetAccount(ManagerServer.Model.PayslipDeductionItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ProfitAndLossStatementAccount>(row.Account) as NamedObject ?? ApplicationData.Businesses.Get(Business).SingleOrDefault<BalanceSheetAccount>(row.Account) as NamedObject ?? ApplicationData.Businesses.Get(Business).Single<BalanceSheetSuspenseAccount>();

        [HideColumnIfAllEmpty]
        [Guid("23000533-d57f-4a6f-981c-3cb3fe90dd1e")]
        [Guide("Optional reporting category used to group similar deduction items for analysis and reporting purposes.")]
        [Guide("Categories help organize deductions into meaningful groups like *Tax Deductions*, *Insurance Deductions*, or *Retirement Contributions* for clearer payroll reports.")]
        public NamedObject GetReportingCategory(ManagerServer.Model.PayslipDeductionItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<PayslipDeductionItemReportingCategory>(row.ReportingCategory);
    }
}
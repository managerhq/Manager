using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipContributionItems
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.PayslipContributionItems))]
    [NewButton(nameof(Strings.NewPayslipItem))]
    [Guide("This screen allows you to manage payslip contribution items. These items can be selected when creating payslips under the **Payslips** tab for your employees.")]
    [Guide("Payslip contribution items represent employer contributions made on behalf of employees that are not paid to the employee directly. These contributions increase the employer's expense without affecting the employee's net pay.")]
    [Guide("Common examples include pension contributions paid directly to an employee's pension fund, health insurance premiums paid to insurance providers, or other statutory contributions required by law.")]
    internal sealed class PayslipContributionItems : PersistentObjectTable<ManagerServer.Model.PayslipContributionItem>
    {
        [Guid("851d676b-50bb-4d03-8138-bf47617d1ffd")]
        public string GetName(ManagerServer.Model.PayslipContributionItem row) => row.Name;

        [Guid("269d74ae-4530-4e6d-a691-f8ed55f65986")]
        public NamedObject GetExpenseAccount(ManagerServer.Model.PayslipContributionItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ProfitAndLossStatementAccount>(row.ExpenseAccount) as NamedObject ?? ApplicationData.Businesses.Get(Business).Single<BalanceSheetSuspenseAccount>();

        [Guid("27ae34bb-d199-4c28-95cb-1930a7dd9b56")]
        public NamedObject GetLiabilityAccount(ManagerServer.Model.PayslipContributionItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<BalanceSheetAccount>(row.LiabilityAccount) as NamedObject ?? ApplicationData.Businesses.Get(Business).Single<BalanceSheetSuspenseAccount>();

        [HideColumnIfAllEmpty]
        [Guid("cb31e625-c4c6-4057-845d-43ea691b5fe4")]
        public NamedObject GetReportingCategory(ManagerServer.Model.PayslipContributionItem row) => ApplicationData.Businesses.Get(Business).SingleOrDefault<PayslipContributionItemReportingCategory>(row.ReportingCategory);
    }
}
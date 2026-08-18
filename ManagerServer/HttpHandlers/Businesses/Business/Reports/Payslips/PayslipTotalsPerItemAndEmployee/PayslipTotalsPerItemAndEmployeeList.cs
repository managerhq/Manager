using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipTotalsPerItemAndEmployee))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`PayslipTotalsPerItemAndEmployee` provides a detailed breakdown of payroll earnings, deducitons and contributions, summarizing the total amounts for each payslip item and categorizing them by individual employee.")]
    [Guide("To create a new `PayslipTotalsPerItemAndEmployee`, go to `Reports` tab, click `PayslipTotalsPerItemAndEmployee`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.PayslipTotalsPerItemAndEmployee), name: nameof(Strings.NewReport))]
    internal sealed class PayslipTotalsPerItemAndEmployeeList : PersistentObjectTable<ManagerServer.Model.PayslipTotalsPerItemAndEmployee>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("5b6f4c0f-3f57-4dbf-9331-ae7650f660ba")]
        public DateTime? GetFromDate(ManagerServer.Model.PayslipTotalsPerItemAndEmployee o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("d8dd99a0-2efc-48a5-a4d5-4cda42fd2dc9")]
        public DateTime? GetToDate(ManagerServer.Model.PayslipTotalsPerItemAndEmployee o) => o.Periods?[0].ToDate;

        [Guid("86d243b8-413e-44ba-a60a-51f35823c605")]
        public string GetDescription(ManagerServer.Model.PayslipTotalsPerItemAndEmployee o) => o.Description;
    }
}
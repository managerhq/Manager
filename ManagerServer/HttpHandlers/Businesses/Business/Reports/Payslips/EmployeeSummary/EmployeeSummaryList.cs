using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.EmployeeSummary
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`EmployeeSummary` provides a comprehensive overview of employee playslips, allowing you to see earnings, deductions and contributions over period of time.")]
    [Guide("To create a new `EmployeeSummary`, go to `Reports` tab, click `EmployeeSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.EmployeeSummary), name: nameof(Strings.NewReport))]
    internal sealed class EmployeeSummaryList : PersistentObjectTable<ManagerServer.Model.EmployeeSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("7f9fbe65-88c6-47b0-bc26-5a68a23e75db")]
        public DateTime GetFromDate(ManagerServer.Model.EmployeeSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("03ccf643-5928-4cdb-ba0f-8cdde07ddd8c")]
        public DateTime GetToDate(ManagerServer.Model.EmployeeSummary o) => o.ToDate;

        [Guid("2ee87dad-a6d7-4cf9-af2d-8f82fc2fa27d")]
        public ManagerServer.Model.Employee GetEmployee(ManagerServer.Model.EmployeeSummary o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Employee>(o.Employee);
    }
}
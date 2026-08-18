using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedReceivables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedReceivables))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`AgedReceivables` provides a comprehensive overview of outstanding invoices, helping you track overdue payments and manage your accounts receivable more effectively.")]
    [Guide("To create a new `AgedReceivables`, go to `Reports` tab, click `AgedReceivables`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.AgedReceivables), name: nameof(Strings.NewReport))]
    internal sealed class AgedReceivablesList : PersistentObjectTable<ManagerServer.Model.AgedReceivables>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("e3d66988-14ef-4a6c-9d78-0a3fc5dd4538")]
        public DateTime GetDate(ManagerServer.Model.AgedReceivables o) => o.Date == DateType.Today ? DateTime.Today : o.CustomDate;

        [HideColumnIfAllEmpty]
        [Guid("dcca0a25-e90c-484a-b24a-8d89386ec5cd")]
        public ManagerServer.Model.Division GetDivision(ManagerServer.Model.AgedReceivables o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Division>(o.Division);

        [Guid("be112b52-e6a3-4f60-bb32-40fff1995a6a")]
        public string GetDescription(ManagerServer.Model.AgedReceivables o) => o.Description;
    }
}
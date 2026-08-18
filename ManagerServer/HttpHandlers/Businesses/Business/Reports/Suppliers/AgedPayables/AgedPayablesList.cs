using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedPayables))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Aged Payables* report provides a detailed breakdown of your outstanding supplier invoices, organized by the length of time they have remained unpaid.")]
    [Guide("This report helps you monitor your payment obligations and identify overdue invoices that require immediate attention.")]
    [Guide("To create a new report, go to the **Reports** tab, click **Aged Payables**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.AgedPayables), name: nameof(Strings.NewReport))]
    internal sealed class AgedPayablesList : PersistentObjectTable<ManagerServer.Model.AgedPayables>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("ddcd474b-5b20-430b-b06b-41eb14d42ed2")]
        public DateTime GetDate(ManagerServer.Model.AgedPayables o) => o.Date == DateType.Today ? DateTime.Today : o.CustomDate;

        [HideColumnIfAllEmpty]
        [Guid("bac9fda8-90a7-4337-857d-5eef6dcaaacb")]
        public ManagerServer.Model.Division GetDivision(ManagerServer.Model.AgedPayables o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Division>(o.Division);

        [Guid("1299e039-bb91-4d13-b9a3-380a34bb3937")]
        public string GetDescription(ManagerServer.Model.AgedPayables o) => o.Description;
    }
}
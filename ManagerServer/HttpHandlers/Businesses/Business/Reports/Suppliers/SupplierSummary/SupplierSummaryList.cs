using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierSummary
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Supplier Summary* report provides a comprehensive overview of all transactions and balances with your suppliers, allowing you to easily monitor outstanding invoices, payments made, and overall financial relationships with each supplier.")]
    [Guide("To create a new supplier summary report, go to the **Reports** tab, click **Supplier Summary**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SupplierSummary), name: nameof(Strings.NewReport))]
    internal sealed class SupplierSummaryList : PersistentObjectTable<ManagerServer.Model.SupplierSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("a7fcf91b-ed63-48ef-bac5-24a6d77026db")]
        public DateTime GetFromDate(ManagerServer.Model.SupplierSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("1b961b16-8171-41be-995b-f31dfff764aa")]
        public DateTime GetToDate(ManagerServer.Model.SupplierSummary o) => o.ToDate;

        [Guid("a9df38ff-87e9-4dbb-9583-aaa4d90692fb")]
        public ManagerServer.Model.Division GetDivison(ManagerServer.Model.SupplierSummary o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Division>(o.Division);

        [Guid("cae82aec-8d97-4cd5-a5a0-791511f8903c")]
        public string GetDescription(ManagerServer.Model.SupplierSummary o) => string.Empty;
    }
}
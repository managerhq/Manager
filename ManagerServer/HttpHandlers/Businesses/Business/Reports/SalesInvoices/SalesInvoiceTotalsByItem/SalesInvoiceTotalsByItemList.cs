using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByItem))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`SalesInvoiceTotalsByItem` provides a detailed breakdown of the total sales amounts for each item sold.")]
    [Guide("To create a new `SalesInvoiceTotalsByItem`, go to `Reports` tab, click `SalesInvoiceTotalsByItem`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SalesInvoiceTotalsByItem), name: nameof(Strings.NewReport))]
    internal sealed class SalesInvoiceTotalsByItemList : PersistentObjectTable<ManagerServer.Model.SalesInvoiceTotalsByItem>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("1e273c7d-711c-4a99-b085-570c1faa3202")]
        public DateTime? GetFromDate(ManagerServer.Model.SalesInvoiceTotalsByItem o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("3e5f75e9-badb-4b71-a6ca-3066144fa032")]
        public DateTime? GetToDate(ManagerServer.Model.SalesInvoiceTotalsByItem o) => o.Periods?[0].ToDate;

        [Guid("aa269f8f-543f-46ca-9d70-c13aa1ea861f")]
        public string GetDescription(ManagerServer.Model.SalesInvoiceTotalsByItem o) => o.Description;
    }
}
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomer))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`SalesInvoiceTotalsByCustomer` provides a comprehensive summary of all sales invoices grouped by each customer for the period of time.")]
    [Guide("To create a new `SalesInvoiceTotalsByCustomer`, go to `Reports` tab, click `SalesInvoiceTotalsByCustomer`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SalesInvoiceTotalsByCustomer), name: nameof(Strings.NewReport))]
    internal sealed class SalesInvoiceTotalsByCustomerList : PersistentObjectTable<ManagerServer.Model.SalesInvoiceTotalsByCustomer>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("e53c45dc-ffe8-4efe-9a0b-5a3ec8fc6c8b")]
        public DateTime? GetFromDate(ManagerServer.Model.SalesInvoiceTotalsByCustomer o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("4eb2f4e6-1210-439b-9690-64ba8cef1729")]
        public DateTime? GetToDate(ManagerServer.Model.SalesInvoiceTotalsByCustomer o) => o.Periods?[0].ToDate;

        [Guid("c0e30c40-9c76-419b-9e86-b9f5a06ff931")]
        public string GetDescription(ManagerServer.Model.SalesInvoiceTotalsByCustomer o) => o.Description;
    }
}
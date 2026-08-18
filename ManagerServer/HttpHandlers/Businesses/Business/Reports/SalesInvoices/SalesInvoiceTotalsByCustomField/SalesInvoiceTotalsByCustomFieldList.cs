using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomField))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`SalesInvoiceTotalsByCustomField` provides a detailed breakdown of your sales invoices, categorized by custom fields, allowing for enhanced analysis and tracking of specific data points tailored to your business needs.")]
    [Guide("To create a new `SalesInvoiceTotalsByCustomField`, go to `Reports` tab, click `SalesInvoiceTotalsByCustomField`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SalesInvoiceTotalsByCustomField), name: nameof(Strings.NewReport))]
    internal sealed class SalesInvoiceTotalsByCustomFieldList : PersistentObjectTable<ManagerServer.Model.SalesInvoiceTotalsByCustomField>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("9c22a069-52f5-4ce7-838c-7deec95d96a7")]
        public DateTime? GetFromDate(ManagerServer.Model.SalesInvoiceTotalsByCustomField o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("23f4a1a1-c40e-4c04-843f-413d13c05b54")]
        public DateTime? GetToDate(ManagerServer.Model.SalesInvoiceTotalsByCustomField o) => o.Periods?[0].ToDate;

        [Guid("442f9017-2dd7-45ca-9c3e-084cd083f517")]
        public string GetName(ManagerServer.Model.SalesInvoiceTotalsByCustomField o) => o.Name;
    }
}
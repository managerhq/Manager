using ManagerServer.Model;
using Markdig;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoice), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays a preview of how your *sales invoice footer* will appear on printed and emailed invoices.")]
    [Guide("The footer text shown here will be automatically added to the bottom of all your sales invoices, providing consistent information such as payment terms, bank details, or legal notices.")]
    [LinkGuide("To edit the footer content, see:", typeof(SalesInvoiceFooterForm))]
    internal sealed class SalesInvoiceFooterView : DefaultView<GetSalesInvoiceFooterView>
    {
    }
}
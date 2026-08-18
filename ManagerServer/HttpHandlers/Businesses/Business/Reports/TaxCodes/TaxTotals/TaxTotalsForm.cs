using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTotals
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTotals))]
    [Guide("The **Tax Totals** report shows total tax figures for a specified period, broken down by *tax code* and by *tax code component*.")]
    [Guide("This report is useful when tax codes combine multiple components, such as federal and state taxes, and you need totals for each component separately.")]
    [Header("Report Configuration")]
    [Guide("Configure the report parameters below to generate tax totals for your desired period and accounting method.")]
    [Guide("You can filter by *division* if you need tax information for specific segments of your business.")]
    [Fields(typeof(ManagerServer.Model.TaxTotals))]
    internal sealed class TaxTotalsForm : NakedVueForm<ManagerServer.Model.TaxTotals>
    {
    }
}

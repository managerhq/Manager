using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxTotals;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTotals
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTotals))]
    [Guide("The **Tax Totals** report shows total tax figures for the period, broken down by *tax code* and by *tax code component*.")]
    [Header("Report Contents")]
    [Guide("The report displays the following information for each *tax code*:")]
    [Guide("• *Tax Exclusive Total* - Net amounts before tax (sales minus purchases)")]
    [Guide("• *Tax Amount* - Net tax for the period (tax collected minus tax paid)")]
    [Guide("• *Tax Inclusive Total* - Combined net amounts and tax")]
    [Guide("Tax codes with multiple components show a row for each component, so compound taxes such as federal + state taxes can be reviewed per component.")]
    [Header("Using the Report")]
    [Guide("The report can be generated using either *accrual basis* or *cash basis* accounting methods, depending on your reporting requirements.")]
    [LinkGuide("To customize this report, see:", typeof(TaxTotalsForm))]
    internal sealed class TaxTotalsView : DefaultView<GetTaxTotalsView>
    {
    }
}

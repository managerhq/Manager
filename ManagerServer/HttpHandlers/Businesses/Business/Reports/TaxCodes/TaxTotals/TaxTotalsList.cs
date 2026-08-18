using System;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTotals
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTotals))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The **Tax Totals** report provides total tax figures for a specific period, broken down by *tax code* and by *tax code component*.")]
    [Guide("For each tax code, the report shows the tax exclusive total, tax amount, and tax inclusive total.")]
    [Guide("Tax codes with multiple components show each component separately, which is useful for compound taxes like federal + state taxes remitted to different tax authorities.")]
    [Guide("To create a new tax totals report, go to the **Reports** tab, click **Tax Totals**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxTotals), name: nameof(Strings.NewReport))]
    internal sealed class TaxTotalsList : PersistentObjectTable<ManagerServer.Model.TaxTotals>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("cb6d6677-bf55-4737-9d4d-93f52816f7cc")]
        public DateTime GetFromDate(ManagerServer.Model.TaxTotals o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b83891da-53a6-4bb0-b48d-521747727419")]
        public DateTime GetToDate(ManagerServer.Model.TaxTotals o) => o.ToDate;

        [Guid("e371cb0e-466d-4f3e-bf9f-ecc2cebf104d")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxTotals o) => o.AccountingMethod;

        [Guid("72bfe109-6a29-4b76-b0bb-535550ebeb72")]
        public string GetDescription(ManagerServer.Model.TaxTotals o) => o.Description;
    }
}

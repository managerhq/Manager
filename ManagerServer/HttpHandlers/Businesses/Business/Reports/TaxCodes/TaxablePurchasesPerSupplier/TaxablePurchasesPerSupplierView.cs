using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxablePurchasesPerSupplier;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    [Title(nameof(Strings.TaxablePurchasesPerSupplier))]
    [Guide("The **Taxable Purchases Per Supplier** report provides a breakdown of tax amounts organized by supplier for your specified reporting period.")]
    [Guide("This report is essential for tax compliance, helping you track and verify the tax collected on purchases from each supplier.")]
    [Header("What This Report Shows")]
    [Guide("For each supplier and tax code combination, the report displays three key amounts:")]
    [Guide("• **Net Purchases** - The purchase amounts before tax")]
    [Guide("• **Tax on Purchases** - The tax amounts charged by suppliers")]
    [Guide("• **Total Purchases** - The combined total of net purchases plus tax")]
    [Header("Report Organization")]
    [Guide("Transactions are grouped by *tax code*, with suppliers listed alphabetically within each tax code group.")]
    [Guide("This organization makes it easy to see which tax codes apply to purchases from specific suppliers and helps identify tax patterns across your supplier base.")]
    [LinkGuide("For configuration options, see:", typeof(TaxablePurchasesPerSupplierForm))]
    internal sealed class TaxablePurchasesPerSupplierView : DefaultView<GetTaxablePurchasesPerSupplierView>
    {
    }
}
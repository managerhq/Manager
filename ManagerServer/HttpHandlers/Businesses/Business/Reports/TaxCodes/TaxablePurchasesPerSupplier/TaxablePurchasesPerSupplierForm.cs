using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    [Title(nameof(Strings.TaxablePurchasesPerSupplier))]
    [Guide("The **Taxable Purchases Per Supplier** report shows a breakdown of taxable purchases organized by supplier for a specified period.")]
    [Guide("This report helps you track tax obligations by analyzing purchase transactions that include tax codes, making it useful for tax reporting and compliance purposes.")]
    [Header("Report Configuration")]
    [Guide("Select the date range to analyze purchases within a specific period. The report will include all taxable purchase transactions that fall within your selected dates.")]
    [Guide("Choose which *tax codes* to include in the report. You can select multiple tax codes to see a comprehensive view of different tax types, or focus on specific codes for detailed analysis.")]
    [Fields(typeof(ManagerServer.Model.TaxablePurchasesPerSupplier))]
    internal sealed class TaxablePurchasesPerSupplierForm : NakedVueForm<ManagerServer.Model.TaxablePurchasesPerSupplier>
    {
    }
}

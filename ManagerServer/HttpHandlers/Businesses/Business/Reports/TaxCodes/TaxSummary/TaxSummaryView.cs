using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    [Title(nameof(Strings.TaxSummary))]
    [Guide("The **Tax Summary** report provides a comprehensive overview of tax collected on sales and tax paid on purchases during a specified period.")]
    [Guide("This report is essential for understanding your tax position and preparing tax returns.")]
    [Header("Report Contents")]
    [Guide("The report displays the following information for each *tax code*:")]
    [Guide("• *Net Sales* - Sales amounts before tax")]
    [Guide("• *Tax on Sales* - Tax amounts collected from customers")]
    [Guide("• *Total Sales* - Combined sales and tax amounts")]
    [Guide("• *Net Purchases* - Purchase amounts before tax")]
    [Guide("• *Tax on Purchases* - Tax amounts paid to suppliers")]
    [Guide("• *Total Purchases* - Combined purchases and tax amounts")]
    [Guide("• *Tax Liability* - The difference between tax collected and tax paid")]
    [Header("Using the Report")]
    [Guide("Click on any *Net Sales* or *Net Purchases* amount to view the underlying transactions that make up that figure.")]
    [Guide("The report can be generated using either *accrual basis* or *cash basis* accounting methods, depending on your reporting requirements.")]
    [LinkGuide("To customize this report, see:", typeof(TaxSummaryForm))]
    internal sealed class TaxSummaryView : DefaultView<GetTaxSummaryView>
    {
    }
}
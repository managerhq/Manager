using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    [Title(nameof(Strings.TaxSummary))]
    [Guide("The **Tax Summary** report provides a comprehensive overview of all tax transactions within a specified period.")]
    [Guide("This report groups transactions by *tax code*, showing both tax collected from sales and tax paid on purchases.")]
    [Header("Purpose and Benefits")]
    [Guide("Use this report to analyze your tax position for each *tax code* in your system.")]
    [Guide("The report helps you understand your net tax liability or refund position by comparing tax collected versus tax paid.")]
    [Guide("This information is essential for preparing tax returns and ensuring compliance with tax regulations.")]
    [Header("Report Configuration")]
    [Guide("Configure the report parameters below to generate a tax summary for your desired period and accounting method.")]
    [Guide("You can filter by *division* if you need tax information for specific segments of your business.")]
    [Fields(typeof(ManagerServer.Model.TaxSummary))]
    internal sealed class TaxSummaryForm : NakedVueForm<ManagerServer.Model.TaxSummary>
    {
    }
}

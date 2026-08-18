using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierSummary
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierSummary))]
    [Guide("The **Supplier Summary** report provides a comprehensive overview of all your suppliers and their account balances for a specified period.")]
    [Guide("This report helps you monitor outstanding payables, track payment history, and analyze supplier relationships over time.")]
    [Header("Report Configuration")]
    [Guide("To generate the report, specify the date range you want to analyze. The report will show opening balances, total purchases, payments made, and closing balances for each supplier.")]
    [Guide("You can optionally filter the report by *division* to focus on suppliers associated with specific business segments or locations.")]
    [Header("Understanding the Report")]
    [Guide("The report displays each supplier with columns showing their opening balance at the start date, total purchases during the period, payments made, and the closing balance at the end date.")]
    [Guide("Use this information to identify suppliers with large outstanding balances, review payment patterns, and ensure your records match supplier statements.")]
    [Fields(typeof(ManagerServer.Model.SupplierSummary))]
    internal sealed class SupplierSummaryForm : NakedVueForm<ManagerServer.Model.SupplierSummary>
    {
    }
}
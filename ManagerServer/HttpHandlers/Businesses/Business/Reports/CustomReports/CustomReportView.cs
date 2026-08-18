using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomReports
{
    [ProtoContract]
    [Title(nameof(Strings.CustomReport))]
    [Guide("The Custom Report view displays the results of your custom report. This view presents your data in a formatted table with the columns, grouping, and calculations you configured.")]
    [Guide("Your report will display the business name at the top, followed by the report name and the date range you selected. The data appears in a structured table format that respects your chosen layout settings.")]
    [Header("Report Features")]
    [Guide("At the top of the report, you'll find the **Edit** button to modify your report configuration and the **Clone** button to create a copy of this report with a new name.")]
    [Guide("The report automatically calculates totals for numeric columns when you use grouping. These totals appear at the end of each group and at the bottom of the report.")]
    [Guide("You can print or email the report using the buttons in the top toolbar. The report will be formatted appropriately for printing with your business details included.")]
    [Header("Data Organization")]
    [Guide("If you configured grouping in your report, data will be organized hierarchically with group headers and subtotals. Groups can be nested multiple levels deep, with each level indented for clarity.")]
    [Guide("When the *Collapse groups* option is enabled in your report configuration, the lowest level of grouping will be collapsed into summary rows showing totals rather than individual transactions.")]
    [LinkGuide("To learn more about creating and configuring custom reports, see:", typeof(CustomReportForm))]
    internal sealed class CustomReportView : DefaultView<ManagerServer.Api.Businesses.Business.Reports.CustomReports.GetCustomReportView>
    {
    }
}

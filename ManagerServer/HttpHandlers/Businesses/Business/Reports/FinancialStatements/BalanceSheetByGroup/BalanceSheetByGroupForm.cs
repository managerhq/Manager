using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheetByGroup
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheetByGroup), nameof(Strings.Edit))]
    [Guide("The `BalanceSheetByGroup` report shows the contents of a single balance sheet group at a specific date. Use it alongside the main `BalanceSheet` to drill into a group that has been collapsed on the main report.")]
    [Guide("Configure the report using these options:")]
    [Fields(typeof(ManagerServer.Model.BalanceSheetByGroup))]
    [Guide("The `Group` field determines which balance sheet group is rendered. Subgroups and accounts under that group will appear on the report.")]
    [LinkGuide("For more information see:", typeof(BalanceSheet.BalanceSheetForm))]
    internal sealed class BalanceSheetByGroupForm : NakedVueForm<ManagerServer.Model.BalanceSheetByGroup>
    {
    }
}

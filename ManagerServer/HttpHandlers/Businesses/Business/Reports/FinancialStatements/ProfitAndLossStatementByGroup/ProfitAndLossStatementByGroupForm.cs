using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementByGroup
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementByGroup), nameof(Strings.Edit))]
    [Guide("The `ProfitAndLossStatementByGroup` report shows the contents of a single profit & loss group over a period of time. Use it alongside the main `ProfitAndLossStatement` to drill into a group that has been collapsed on the main report.")]
    [Guide("Configure the report using these options:")]
    [Fields(typeof(ManagerServer.Model.ProfitAndLossStatementByGroup))]
    [Guide("The `Group` field determines which profit & loss group is rendered. Subgroups and accounts under that group will appear on the report.")]
    [LinkGuide("For more information see:", typeof(ProfitAndLossStatement.ProfitAndLossStatementForm))]
    internal sealed class ProfitAndLossStatementByGroupForm : NakedVueForm<ManagerServer.Model.ProfitAndLossStatementByGroup>
    {
    }
}

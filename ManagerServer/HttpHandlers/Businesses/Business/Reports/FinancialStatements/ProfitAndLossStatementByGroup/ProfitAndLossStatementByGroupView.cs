using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementByGroup;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementByGroup
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementByGroup))]
    [Guide("The `ProfitAndLossStatementByGroup` report shows the contents of a single profit & loss group over a period of time.")]
    [LinkGuide("For more information see:", typeof(ProfitAndLossStatementByGroupForm))]
    internal sealed class ProfitAndLossStatementByGroupView : DefaultView<GetProfitAndLossStatementByGroupView>
    {
    }
}

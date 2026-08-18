using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.BalanceSheetByGroup;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheetByGroup
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheetByGroup))]
    [Guide("The `BalanceSheetByGroup` report shows the contents of a single balance sheet group at a specific date.")]
    [LinkGuide("For more information see:", typeof(BalanceSheetByGroupForm))]
    internal sealed class BalanceSheetByGroupView : DefaultView<GetBalanceSheetByGroupView>
    {
    }
}

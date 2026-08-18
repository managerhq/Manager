using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.History))]
    [Guide("This screen shows individual event within **History**")]
    internal sealed class ChangeView : DefaultView<ManagerServer.Api.Businesses.Business.History.GetChangeView>
    {
    }
}

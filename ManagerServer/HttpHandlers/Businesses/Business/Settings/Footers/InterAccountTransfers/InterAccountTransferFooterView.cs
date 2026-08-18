using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.InterAccountTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.InterAccountTransfer), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays your current *inter-account transfer footer* configuration, showing exactly how it will appear on printed or emailed inter-account transfer documents.")]
    [Guide("Use the preview section to verify that your footer content, formatting, and layout meet your requirements before applying it to actual transfers.")]
    [LinkGuide("To modify the footer content, see:", typeof(InterAccountTransferFooterForm))]
    internal class InterAccountTransferFooterView : DefaultView<GetInterAccountTransferFooterView>
    {
    }
}

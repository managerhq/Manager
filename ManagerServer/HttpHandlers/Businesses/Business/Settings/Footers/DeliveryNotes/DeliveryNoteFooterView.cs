using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.DeliveryNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DeliveryNote), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("The delivery note footer view displays the current footer text that appears at the bottom of all delivery notes.")]
    [Guide("Use this screen to preview how your footer will appear on printed or emailed delivery notes before making changes.")]
    [Guide("The footer typically contains important information such as terms and conditions, return policies, or contact details that you want to include on every delivery note.")]
    [LinkGuide("To edit the footer content, see:", typeof(DeliveryNoteFooterForm))]
    internal sealed class DeliveryNoteFooterView : DefaultView<GetDeliveryNoteFooterView>
    {
    }
}

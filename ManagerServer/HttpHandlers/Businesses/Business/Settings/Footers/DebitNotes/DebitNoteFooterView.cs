using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.DebitNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DebitNote), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("The debit note footer appears at the bottom of every debit note you send to suppliers. This page allows you to view your current footer settings and see how they will appear on printed or emailed debit notes.")]
    [Guide("Use the **Edit** button to modify the footer content, or click **Print** to see a preview of how the footer will look on an actual debit note.")]
    [LinkGuide("To customize the footer content, see:", typeof(DebitNoteFooterForm))]
    internal sealed class DebitNoteFooterView : DefaultView<GetDebitNoteFooterView>
    {
    }
}
using ManagerServer.Model;
using Markdig;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.CreditNotes
{
    [ProtoContract]
    [Title(nameof(Strings.CreditNote), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays your current *credit note footer* configuration and shows a preview of how it will appear on credit notes.")]
    [Guide("The footer appears at the bottom of every credit note you issue and can contain important information such as terms, conditions, or contact details.")]
    [LinkGuide("To edit the footer content, see:", typeof(CreditNoteFooterForm))]
    internal sealed class CreditNoteFooterView : DefaultView<GetCreditNoteFooterView>
    {
    }
}
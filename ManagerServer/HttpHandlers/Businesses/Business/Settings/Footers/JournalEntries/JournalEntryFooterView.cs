using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.JournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.JournalEntry), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays your current *journal entry footer* settings and provides a preview of how the footer will appear on your journal entries.")]
    [Guide("The preview shows exactly how the footer text will be formatted and positioned at the bottom of each journal entry document.")]
    [LinkGuide("To modify footer settings, see:", typeof(JournalEntryFooterForm))]
    internal class JournalEntryFooterView : DefaultView<GetJournalEntryFooterView>
    {
    }
}

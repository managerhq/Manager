using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.JournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of journal entries.")]
    [Guide("Use footers to add terms, conditions, or additional information to journal entries.")]
    [Fields(typeof(ManagerServer.Model.JournalEntryFooter))]
    internal sealed class JournalEntryFooterForm : NakedVueForm<ManagerServer.Model.JournalEntryFooter>
    {
    }
}
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.DebitNotes
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of debit notes.")]
    [Guide("Use footers to add terms, conditions, or additional information to debit notes.")]
    [Fields(typeof(ManagerServer.Model.DebitNoteFooter))]
    internal sealed class DebitNoteFooterForm : NakedVueForm<ManagerServer.Model.DebitNoteFooter>
    {
    }
}
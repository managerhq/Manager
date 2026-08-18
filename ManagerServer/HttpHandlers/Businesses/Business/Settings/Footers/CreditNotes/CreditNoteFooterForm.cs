using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.CreditNotes
{
    [ProtoContract]
    [Title(nameof(Strings.Footer), nameof(Strings.CreditNote), nameof(Strings.Edit))]
    [Guide("Configure footer text that appears at the bottom of credit notes.")]
    [Guide("Use footers to add terms, conditions, or additional information to credit notes.")]
    [Fields(typeof(ManagerServer.Model.CreditNoteFooter))]
    internal sealed class CreditNoteFooterForm : NakedVueForm<ManagerServer.Model.CreditNoteFooter>
    {
    }
}
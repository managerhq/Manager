using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.DeliveryNotes
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of delivery notes.")]
    [Guide("Use footers to add terms, conditions, or additional information to delivery notes.")]
    [Fields(typeof(ManagerServer.Model.DeliveryNoteFooter))]
    internal sealed class DeliveryNoteFooterForm : NakedVueForm<ManagerServer.Model.DeliveryNoteFooter>
    {
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.DeliveryNotes
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(DeliveryNotes))]
    [Title(nameof(Strings.DeliveryNote))]
    [Guide("Delivery note footers are customizable text sections that appear at the bottom of your delivery notes.")]
    [Guide("Create multiple footer templates to use with different types of deliveries or customer requirements.")]
    [Columns]
    internal sealed class DeliveryNoteFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.DeliveryNoteFooter>
    {
        [Default]
        [Guide("Footers appear at the bottom of your delivery notes and typically contain important shipping and handling instructions.")]
        [Header("Common Footer Content")]
        [Guide("Use footers to include essential delivery information such as:")]
        [Guide("• Unloading instructions and equipment requirements")]
        [Guide("• Damage reporting procedures and contact information")]
        [Guide("• Signature requirements and acceptance procedures")]
        [Guide("• Storage conditions and temperature requirements")]
        [Guide("• Return policies and procedures")]
        [Header("Managing Footer Templates")]
        [Guide("You can create multiple footer templates and select the appropriate one when creating each delivery note.")]
        [Guide("This flexibility ensures proper goods handling while accommodating different delivery scenarios or customer requirements.")]
        [Guide("Enter a descriptive name for each footer template to easily identify its purpose, such as 'Fragile Goods Instructions' or 'Standard Delivery Terms'.")]
        public string[] GetName(ManagerServer.Model.DeliveryNoteFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

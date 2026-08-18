using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("ccad5339-206e-4145-aa3d-3bd8d785b2fb")]
    public sealed class CheckboxCustomField : NamedObject, IReportingCategory, ICustomField
    {
        [Guide("Enter the name of this checkbox custom field. This label appears next to the checkbox.")]
        [Guide("Checkbox fields are perfect for yes/no questions, confirmations, or optional features.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Group related checkboxes together using consecutive position numbers.")]
        [ProtoMember(2)] public int? Position { get; set; }
        [Guide("Select where this checkbox should appear. You can assign it to multiple forms or line items.")]
        [Guide("Examples: 'Tax exempt' on customers, 'Requires approval' on purchases, 'Fragile' on inventory items.")]
        [ProtoMember(3), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Enter help text that appears below the checkbox. This guides users on when to check this box.")]
        [Guide("Clearly explain what checking the box means, like 'Check if customer is tax exempt' or 'Check if item requires special handling'.")]
        [ProtoMember(4), Textarea] public string Description { get; set; }
        [Guide("Check this box to prevent this checkbox value from being copied when duplicating records.")]
        [Guide("Use for checkboxes that should be reconsidered for each record, like approval status.")]
        [ProtoMember(9)] public bool ExcludeFromCopyingOrCloning { get; set; }
        [Guide("Check this box to prevent this field's value from being edited by users.")]
        [Guide("This is typically used when custom field is updated by extension rather than users directly.")]
        [ProtoMember(10)] public bool LockedForManualEditing { get; set; }
        [Guide("Check this box to show the checkbox status on the View screen after saving.")]
        [Guide("Display important flags or statuses so users can see them without editing the record.")]
        [ProtoMember(6)] public bool DisplayOnView { get; set; }
        [Guide("If displayed on View, check this box to show the checkbox at the top of the View screen.")]
        [Guide("Place critical status indicators at the top, like 'Approved', 'On hold', or 'Urgent'.")]
        [ProtoMember(7), IfTrue(nameof(DisplayOnView))] public bool ShowAtTheTop { get; set; }
        [Guide("Check this box to deactivate this checkbox field. It won't appear on new forms but existing data is preserved.")]
        [Guide("The checkbox values in existing records remain unchanged and can be viewed in reports.")]
        [ProtoMember(8)] public bool Inactive { get; set; }

        [ProtoMember(5)] public bool Obsolete_DisplayOnList { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (Placement == null) return false;
            if (Placement.Length == 0) return false;
            if (filter is ManagerServer.Model.SalesInvoiceTotalsByCustomField)
            {
                if (Placement.Contains(GuidAttribute.GetGuidByType(typeof(ManagerServer.Model.SalesInvoice)))) return true;
                if (Placement.Contains(GuidAttribute.GetGuidByType(typeof(ManagerServer.Model.SalesInvoice.Line)))) return true;
                if (Placement.Contains(GuidAttribute.GetGuidByType(typeof(ManagerServer.Model.Customer)))) return true;
                if (Placement.Contains(GuidAttribute.GetGuidByType(typeof(ManagerServer.Model.InventoryItem)))) return true;
                if (Placement.Contains(GuidAttribute.GetGuidByType(typeof(ManagerServer.Model.NonInventoryItem)))) return true;
            }
            else if (filter != null)
            {
                if (!Placement.Contains(filter.Key)) return false;
            }
            return true;
        }

        public override bool IsInactive() => Inactive;

        public bool ContainsGeneralLedgerTransactions => false;

        string ICustomField.Name => Name;
        int? ICustomField.Position => Position;
        Guid[] ICustomField.Placement => Placement;
        string ICustomField.Description => Description;
        bool ICustomField.DisplayOnView => DisplayOnView;
        bool ICustomField.ShowAtTheTop => ShowAtTheTop;
        bool ICustomField.Inactive => Inactive;
        bool ICustomField.ExcludeFromCopyingOrCloning => ExcludeFromCopyingOrCloning;
        bool ICustomField.LockedForManualEditing => LockedForManualEditing;
    }
}

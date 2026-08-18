using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("411f5975-0376-4ba9-b7ff-5bb2baa6f69f")]
    public sealed class TextCustomField : NamedObject, IReportingCategory, ICustomField
    {
        [Guide("Enter the name of this text custom field. This label appears on forms and reports.")]
        [Guide("Text fields are ideal for capturing textual information like reference numbers, descriptions, or notes.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Fields are sorted by position, then alphabetically. Use gaps like 10, 20, 30 for flexibility.")]
        [ProtoMember(5)] public int? Position { get; set; }
        [Guide("Select where this text field should appear. You can assign it to multiple forms or line items.")]
        [Guide("Text fields can capture reference numbers on invoices, notes on customers, or descriptions on inventory items.")]
        [ProtoMember(13), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Select the type: Single line text, Paragraph text, or Dropdown list.")]
        [Guide("Single line is for short text, Paragraph for multi-line entries, Dropdown for predefined choices.")]
        [ProtoMember(3), TableColumn] public TextCustomFieldType Type { get; set; }
        [Guide("For dropdown lists, enter the options one per line. Users will select from these predefined choices.")]
        [Guide("Use dropdowns when you want to standardize entries, like status codes, categories, or regions.")]
        [ProtoMember(9), Textarea, IfEnum(nameof(Type), (int)TextCustomFieldType.DropdownList)] public string OptionsForDropdownList { get; set; }
        [Guide("Select the field size: Small, Medium, or Large. This controls the width on forms.")]
        [Guide("The size affects visual layout only - all sizes can store unlimited text.")]
        [ProtoMember(4)] public CustomFieldSize Size { get; set; }
        [Guide("Enter help text that appears below the field. This guides users on what information to enter.")]
        [Guide("Examples: 'Enter the customer's purchase order number' or 'Select the appropriate department code'.")]
        [ProtoMember(11), Textarea] public string Description { get; set; }
        [Guide("Check this box to prevent this field's value from being copied when duplicating records.")]
        [Guide("Use this for unique values like serial numbers or reference codes that shouldn't be duplicated.")]
        [ProtoMember(14)] public bool ExcludeFromCopyingOrCloning { get; set; }
        [Guide("Check this box to prevent this field's value from being edited by users.")]
        [Guide("This is typically used when custom field is updated by extension rather than users directly.")]
        [ProtoMember(15)] public bool LockedForManualEditing { get; set; }
        [Guide("Check this box to show the text field value on the View screen after saving.")]
        [Guide("Enable this for important text that users need to see without editing the record.")]
        [ProtoMember(8)] public bool DisplayOnView { get; set; }
        [Guide("If displayed on View, check this box to show the field at the top of the View screen.")]
        [Guide("Reserve top placement for the most critical information like status or approval codes.")]
        [ProtoMember(12), IfTrue(nameof(DisplayOnView))] public bool ShowAtTheTop { get; set; }
        [Guide("Check this box to deactivate this text field. It won't appear on new forms but existing data is preserved.")]
        [Guide("Deactivated fields remain in the system and can be reactivated if needed later.")]
        [ProtoMember(10)] public bool Inactive { get; set; }

        [ProtoMember(7)] public bool Obsolete_DisplayOnList { get; set; }

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

        string ICustomField.Name => Name;
        int? ICustomField.Position => Position;
        Guid[] ICustomField.Placement => Placement;
        string ICustomField.Description => Description;
        bool ICustomField.DisplayOnView => DisplayOnView;
        bool ICustomField.ShowAtTheTop => ShowAtTheTop;
        bool ICustomField.Inactive => Inactive;
        bool ICustomField.ExcludeFromCopyingOrCloning => ExcludeFromCopyingOrCloning;
        bool ICustomField.LockedForManualEditing => LockedForManualEditing;

        public override bool IsInactive() => Inactive;

        public bool ContainsGeneralLedgerTransactions => false;
    }
}

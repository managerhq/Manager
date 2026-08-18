using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Model.Enums;
using System.Reflection;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("dcb382dc-a4e0-4354-a845-b7d647f610f7")]
    public sealed class CustomField : NamedObject, IReportingCategory
    {
        [Guide("Enter the name of this custom field. This label appears on forms and reports.")]
        [Guide("Choose descriptive names that clearly indicate what information should be entered.")]
        [Guide("Examples: 'Purchase Order Number', 'Project Code', 'Department', 'Customer Reference'.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Use increments of 10 (10, 20, 30) to leave room for inserting fields later without renumbering.")]
        [Guide("Fields with the same position number are sorted alphabetically by name.")]
        [ProtoMember(5)] public int? Position { get; set; }
        [Guide("Select where this custom field should appear. You can assign it to multiple forms or line items.")]
        [Guide("Custom fields can be added to transactions, customers, suppliers, inventory items, and many other areas.")]
        [Guide("Hold Ctrl (Cmd on Mac) to select multiple placements. The field will appear on all selected forms.")]
        [ProtoMember(13), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Select the type of custom field: Text, Paragraph text, Dropdown list, Date, Number, Amount, or Image.")]
        [Guide("Choose the type that matches your data: Text for short entries, Paragraph for longer text, Dropdown for standardized choices.")]
        [Guide("Number fields can show totals, Date fields show calendar picker, Image fields allow file uploads.")]
        [ProtoMember(3), TableColumn] public CustomFieldStyle Type { get; set; }
        [Guide("For dropdown lists, enter the options one per line. Users will select from these predefined choices.")]
        [Guide("Dropdown lists ensure consistent data entry and make reporting easier by standardizing values.")]
        [Guide("Example: For a 'Priority' field, enter: High\nMedium\nLow")]
        [ProtoMember(9), Textarea, IfEnum(nameof(Type), (int)CustomFieldStyle.DropdownList)] public string OptionsForDropdownList { get; set; }
        [Guide("Select the field size: Small, Medium, or Large. This controls the width on forms.")]
        [Guide("Small is good for codes or short values, Medium for typical text, Large for longer descriptions.")]
        [Guide("Size only affects display width - it doesn't limit the amount of text that can be entered.")]
        [ProtoMember(4)] public CustomFieldSize Size { get; set; }
        [Guide("Enter help text that appears below the field. This guides users on what information to enter.")]
        [Guide("Good help text includes examples, format requirements, or explains why the information is needed.")]
        [Guide("Keep descriptions concise but informative. Users see this text every time they use the form.")]
        [ProtoMember(11), Textarea] public string Description { get; set; }
        [Guide("Check this box to show the custom field value on the View screen after saving.")]
        [Guide("Important custom fields should be displayed so users can quickly see key information without editing.")]
        [Guide("This is useful for reference numbers, status indicators, or other frequently viewed information.")]
        [ProtoMember(8)] public bool DisplayOnView { get; set; }
        [Guide("If displayed on View, check this box to show the field at the top of the View screen.")]
        [Guide("Use this for critical fields that need immediate visibility, like approval status or priority level.")]
        [Guide("Only enable for the most important fields - too many fields at the top reduces clarity.")]
        [ProtoMember(12), IfTrue(nameof(DisplayOnView))] public bool ShowAtTheTop { get; set; }
        [Guide("Check this box to deactivate this custom field. It won't appear on new forms but existing data is preserved.")]
        [Guide("Deactivate rather than delete custom fields to maintain historical data while preventing new entries.")]
        [Guide("Deactivated fields can be reactivated later if needed. Their data remains in existing records.")]
        [ProtoMember(10)] public bool Inactive { get; set; }

        [ProtoMember(7)] public bool Obsolete_DisplayOnList { get; set; }
        [ProtoMember(2)] public Guid? Obsolete_FormType { get; set; }
        [ProtoMember(6)] public string Obsolete_DefaultValue { get; set; }

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

        public bool Contains(Type type)
        {
            if (Placement == null) return false;
            if (Placement.Length == 0) return false;
            if (type == null) return false;
            if (type.GetCustomAttribute<ManagerServer.Model.Attributes.CustomFieldsAttribute>() == null) return false;
            var key = type.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value;
            return Placement.Contains(key);
        }

        public bool ContainsGeneralLedgerTransactions => false;
    }
}

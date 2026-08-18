using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("f32774a9-f740-4c2b-8353-b321576f6166")]
    public sealed class MultipleValueCustomField : NamedObject, IReportingCategory, ICustomField
    {
        [Guide("Enter the name of this multiple value custom field. This label appears on forms and reports.")]
        [Guide("Multiple value fields let users select several options from a predefined list, like tags or categories.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Multiple value fields work well for categorization and can appear anywhere on the form.")]
        [ProtoMember(2)] public int? Position { get; set; }
        [Guide("Select where this multiple value field should appear. You can assign it to multiple forms or line items.")]
        [Guide("Perfect for: product categories on inventory, service types on invoices, tags on customers.")]
        [ProtoMember(3), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Enter the available options. Users can select multiple values from this list.")]
        [Guide("Add each option as a separate entry. Users can select any combination of these options.")]
        [Guide("Examples: For 'Product Features' field, add options like 'Waterproof', 'Recyclable', 'Organic'.")]
        [ProtoMember(4), FirstColumnLabel] public Option[] Options { get; set; }
        [Guide("Enter help text that appears below the field. This guides users on which options to select.")]
        [Guide("Explain whether all applicable options should be selected or if there are limits.")]
        [ProtoMember(5), Textarea] public string Description { get; set; }
        [Guide("Check this box to prevent selected values from being copied when duplicating records.")]
        [Guide("Enable if the selections are unique to each record and shouldn't carry over.")]
        [ProtoMember(10)] public bool ExcludeFromCopyingOrCloning { get; set; }
        [Guide("Check this box to prevent this field's value from being edited by users.")]
        [Guide("This is typically used when custom field is updated by extension rather than users directly.")]
        [ProtoMember(11)] public bool LockedForManualEditing { get; set; }
        [Guide("Check this box to show the selected values on the View screen after saving.")]
        [Guide("Multiple selections will display as a comma-separated list on the View screen.")]
        [ProtoMember(7)] public bool DisplayOnView { get; set; }
        [Guide("If displayed on View, check this box to show the field at the top of the View screen.")]
        [Guide("Use for important categorizations or tags that need immediate visibility.")]
        [ProtoMember(8), IfTrue(nameof(DisplayOnView))] public bool ShowAtTheTop { get; set; }
        [Guide("Check this box to deactivate this multiple value field. It won't appear on new forms but existing data is preserved.")]
        [Guide("Previously selected values remain in records and can still be used in reports and searches.")]
        [ProtoMember(9)] public bool Inactive { get; set; }

        [ProtoMember(6)] public bool Obsolete_DisplayOnList { get; set; }

        [ProtoContract]
        public sealed class Option
        {
            [Guide("Enter the text for this option that users can select.")]
            [Guide("Keep options concise and mutually compatible since multiple can be selected together.")]
            [ProtoMember(1)] public string Value { get; set; }
        }

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

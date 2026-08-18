using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("68e09438-7aa1-4d63-b7d3-ce2dcd052e88")]
    public sealed class NumberCustomField : NamedObject, IReportingCategory, ICustomField
    {
        [Guide("Enter the name of this number custom field. This label appears on forms and reports.")]
        [Guide("Number fields are perfect for quantities, measurements, scores, or any numerical data.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Number fields appearing on line items can be totaled at the bottom of transactions.")]
        [ProtoMember(2)] public int? Position { get; set; }
        [Guide("Select where this number field should appear. You can assign it to multiple forms or line items.")]
        [Guide("When placed on line items, number fields can calculate totals. Use for quantities, weights, or measurements.")]
        [ProtoMember(3), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Enter help text that appears below the field. This guides users on what numbers to enter.")]
        [Guide("Include units of measurement if applicable, like 'Enter weight in kilograms' or 'Number of units'.")]
        [ProtoMember(4), Textarea] public string Description { get; set; }
        [Guide("Check this box to prevent this field's value from being copied when duplicating records.")]
        [Guide("Enable this for values that should be unique or reset to zero in duplicated records.")]
        [ProtoMember(12)] public bool ExcludeFromCopyingOrCloning { get; set; }
        [Guide("Check this box to prevent this field's value from being edited by users.")]
        [Guide("This is typically used when custom field is updated by extension rather than users directly.")]
        [ProtoMember(13)] public bool LockedForManualEditing { get; set; }
        [Guide("Check this box to enforce a minimum number of decimal places for display consistency.")]
        [Guide("This ensures numbers always show the specified decimal places, like 5.00 instead of 5.")]
        [ProtoMember(10), Label(nameof(Strings.MinimalDecimalPlaces))] public bool HasMinimumDecimalPlaces { get; set; }
        [Guide("Enter the minimum number of decimal places to display. Numbers will show trailing zeros to this precision.")]
        [Guide("For example, setting '2' displays 10 as 10.00, useful for maintaining consistent formatting.")]
        [ProtoMember(11), IfTrue(nameof(HasMinimumDecimalPlaces)), NoLabel, Placeholder("2")] public int? MinimumDecimalPlaces { get; set; }
        [Guide("Check this box to show the number field value on the View screen after saving.")]
        [Guide("Important measurements, quantities, or scores should be visible without editing.")]
        [ProtoMember(6)] public bool DisplayOnView { get; set; }
        [Guide("If displayed on View, check this box to show the field at the top of the View screen.")]
        [Guide("Use for critical numbers that need immediate attention, like priority scores or quantity limits.")]
        [ProtoMember(7), IfTrue(nameof(DisplayOnView))] public bool ShowAtTheTop { get; set; }
        [Guide("Check this box to hide the total amount when this field appears on line items.")]
        [Guide("Enable this if the numbers shouldn't be totaled, like reference numbers or non-additive values.")]
        [ProtoMember(9)] public bool HideTotalAmount { get; set; }
        [Guide("Check this box to deactivate this number field. It won't appear on new forms but existing data is preserved.")]
        [Guide("Historical number data remains intact and the field can be reactivated when needed.")]
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

        public decimal? FormatDecimal(decimal? value)
        {
            if (!value.HasValue) return null;

            var minimumDecimalPlaces = 0;
            if (HasMinimumDecimalPlaces)
            {
                minimumDecimalPlaces = 2;
                if (MinimumDecimalPlaces.HasValue && MinimumDecimalPlaces.Value > -1)
                {
                    minimumDecimalPlaces = MinimumDecimalPlaces.Value;
                }
            }

            if (minimumDecimalPlaces == 0) return value.Value;

            var output = value.Value;

            var nums = decimal.GetBits(value.Value);
            var decimals = BitConverter.GetBytes(nums[3])[2];
            if (decimals < minimumDecimalPlaces)
            {
                var diff = minimumDecimalPlaces - decimals;
                for (int i = 0; i < diff; i++)
                {
                    output *= 1.0m;
                    if (i > 30) break;
                }
            }

            return output;
        }

        public bool ContainsGeneralLedgerTransactions => false;

        public override bool IsInactive() => Inactive;

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

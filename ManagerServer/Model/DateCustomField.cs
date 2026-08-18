using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6c564f4c-380c-432e-af3b-2d6514c1891c")]
    public sealed class DateCustomField : NamedObject, IReportingCategory, ICustomField
    {
        [Guide("Enter the name of this date custom field. This label appears on forms and reports.")]
        [Guide("Date fields are ideal for tracking deadlines, expiry dates, milestone dates, or any time-based information.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Consider grouping related date fields together using sequential position numbers.")]
        [ProtoMember(2)] public int? Position { get; set; }
        [Guide("Select where this date field should appear. You can assign it to multiple forms or line items.")]
        [Guide("Common uses: contract dates on customers, expiry dates on inventory, milestone dates on projects.")]
        [ProtoMember(3), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Enter help text that appears below the field. This guides users on what date to enter.")]
        [Guide("Be specific about which date is needed, like 'Enter contract expiry date' or 'Date of last inspection'.")]
        [ProtoMember(4), Textarea] public string Description { get; set; }
        [Guide("Check this box to prevent this field's value from being copied when duplicating records.")]
        [Guide("Enable for dates that should be re-entered fresh, like inspection dates or review dates.")]
        [ProtoMember(9)] public bool ExcludeFromCopyingOrCloning { get; set; }
        [Guide("Check this box to prevent this field's value from being edited by users.")]
        [Guide("This is typically used when custom field is updated by extension rather than users directly.")]
        [ProtoMember(10)] public bool LockedForManualEditing { get; set; }
        [Guide("Check this box to show the date field value on the View screen after saving.")]
        [Guide("Display important dates like expiry dates, due dates, or renewal dates for quick reference.")]
        [ProtoMember(6)] public bool DisplayOnView { get; set; }
        [Guide("If displayed on View, check this box to show the field at the top of the View screen.")]
        [Guide("Place time-sensitive dates at the top, such as expiry dates or critical deadlines.")]
        [ProtoMember(7), IfTrue(nameof(DisplayOnView))] public bool ShowAtTheTop { get; set; }
        [Guide("Check this box to deactivate this date field. It won't appear on new forms but existing data is preserved.")]
        [Guide("Past date entries remain in the system and the field can be reactivated if requirements change.")]
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

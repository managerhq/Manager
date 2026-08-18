using System;
using ManagerServer.Model.Attributes;
using System.Linq;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("0e97e7ae-9e5c-4294-8d52-0f0813d20d52")]
    public sealed class ImageCustomField : NamedObject, IReportingCategory, ICustomField
    {
        [Guide("Enter the name of this image custom field. This label appears on forms and reports.")]
        [Guide("Image fields allow users to upload photos, diagrams, logos, or any visual information.")]
        [ProtoMember(1), NoWrap, TableColumn] public string Name { get; set; }
        [Guide("Enter a position number to control the display order. Lower numbers appear first.")]
        [Guide("Image fields typically work well at the bottom of forms to avoid pushing other fields down.")]
        [ProtoMember(2)] public int? Position { get; set; }
        [Guide("Enter the maximum width in pixels for displaying the image. Default is 400 pixels.")]
        [Guide("Images larger than this width will be scaled down proportionally to fit.")]
        [ProtoMember(9), NoWrap, Placeholder("400")] public int? Width { get; set; }
        [Guide("Enter the maximum height in pixels for displaying the image. Default is 400 pixels.")]
        [Guide("Images taller than this height will be scaled down proportionally to fit.")]
        [ProtoMember(10), Placeholder("400")] public int? Height { get; set; }
        [Guide("Select where this image field should appear. You can assign it to multiple forms or line items.")]
        [Guide("Common uses: product photos on inventory items, signatures on customers, receipts on expenses.")]
        [ProtoMember(3), Autocomplete(typeof(CustomFieldsAttribute)), TableColumn] public Guid[] Placement { get; set; }
        [Guide("Enter help text that appears below the field. This guides users on what images to upload.")]
        [Guide("Specify acceptable formats (JPG, PNG) and purpose, like 'Upload product photo' or 'Attach receipt image'.")]
        [ProtoMember(4), Textarea] public string Description { get; set; }
        [Guide("Check this box to prevent images from being copied when duplicating records.")]
        [Guide("Enable this for unique images like signatures or receipts that shouldn't be duplicated.")]
        [ProtoMember(12)] public bool ExcludeFromCopyingOrCloning { get; set; }
        [Guide("Check this box to prevent this field's value from being edited by users.")]
        [Guide("This is typically used when custom field is updated by extension rather than users directly.")]
        [ProtoMember(13)] public bool LockedForManualEditing { get; set; }
        [Guide("Check this box to show the uploaded image on the View screen after saving.")]
        [Guide("Images will display at the specified width and height dimensions on the View screen.")]
        [ProtoMember(11)] public bool DisplayOnView { get; set; }
        [Guide("Check this box to deactivate this image field. It won't appear on new forms but existing data is preserved.")]
        [Guide("Previously uploaded images remain accessible in existing records and reports.")]
        [ProtoMember(8)] public bool Inactive { get; set; }

        [ProtoMember(5)] public bool Obsolete_DisplayOnList { get; set; }

        public int GetWidth() => Width ?? 400;
        public int GetHeight() => Height ?? 400;

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            return false;
        }

        string ICustomField.Name => Name;
        int? ICustomField.Position => Position;
        Guid[] ICustomField.Placement => Placement;
        string ICustomField.Description => Description;
        bool ICustomField.DisplayOnView => DisplayOnView;
        bool ICustomField.ShowAtTheTop => false;
        bool ICustomField.Inactive => Inactive;
        bool ICustomField.ExcludeFromCopyingOrCloning => ExcludeFromCopyingOrCloning;
        bool ICustomField.LockedForManualEditing => LockedForManualEditing;

        public override bool IsInactive() => Inactive;

        public bool ContainsGeneralLedgerTransactions => false;
    }
}

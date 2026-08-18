using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("fd737085-d270-4749-9274-7b458a2ec740")]
    public sealed class DeliveryNoteFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for delivery notes.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the content that will appear at the bottom of delivery notes. You can include delivery terms, contact information, or any other relevant details.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing delivery notes using this footer will not be affected.")]
        [ProtoMember(3)] public bool Inactive { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override bool IsInactive() => Inactive;
    }
}

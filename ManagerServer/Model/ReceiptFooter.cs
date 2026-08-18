using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("b4053b7c-b07a-45ba-aa14-1e3a624f8565")]
    public sealed class ReceiptFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer. This name will appear in dropdown menus when selecting a footer for receipts.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the content that will appear at the bottom of receipts. You can include thank you notes, store policies, or any other customer information.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Mark this footer as inactive to prevent it from appearing in footer selection dropdowns. Existing receipts using this footer will not be affected.")]
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

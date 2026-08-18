using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6db45d4f-ef92-4cb8-916f-a7c8645384be")]
    public sealed class GoodsReceiptFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer template.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the footer text that will appear at the bottom of goods receipts. You can use formatting and multiple lines.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Check this box to deactivate this footer. It won't appear in selection lists but existing goods receipts will retain it.")]
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

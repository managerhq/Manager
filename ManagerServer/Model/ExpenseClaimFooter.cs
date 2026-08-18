using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("2084f5f4-c650-47c5-88e5-4cd4c29e28ab")]
    public sealed class ExpenseClaimFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer template.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the footer text that will appear at the bottom of expense claims. You can use formatting and multiple lines.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Check this box to deactivate this footer. It won't appear in selection lists but existing expense claims will retain it.")]
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

using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("12479269-1209-4684-8d6a-ccc7a447fd62")]
    public sealed class InterAccountTransferFooter : NamedObject
    {
        [Guide("Enter a name to identify this footer template.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the footer text that will appear at the bottom of inter account transfers. You can use formatting and multiple lines.")]
        [ProtoMember(2), Textarea, Long] public string Content { get; set; }
        [Guide("Check this box to deactivate this footer. It won't appear in selection lists but existing transfers will retain it.")]
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

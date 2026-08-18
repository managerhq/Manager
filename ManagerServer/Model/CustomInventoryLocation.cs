using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("fae8151d-252e-45e3-b1f4-e048075b8983")]
    public sealed class CustomInventoryLocation : NamedObject, ICode
    {
        [Guide("Enter a descriptive name for this inventory location (e.g., 'Main Warehouse', 'Store #1', 'Eastern Distribution Center').")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("Optionally assign a unique code to this location for quick reference in reports and transactions.")]
        [ProtoMember(3), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Check this box to deactivate this location. Inactive locations will not appear in selection lists but historical data will be preserved.")]
        [ProtoMember(2)] public bool Inactive { get; set; }

        string ICode.Code => Code;

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }
    }
}

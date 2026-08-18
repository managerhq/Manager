using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("d63413bc-622e-4e39-86bc-15e95eb4e81c")]
    public sealed class DefaultInventoryLocation : NamedObject, ICode
    {
        [Guide("Optionally rename the default inventory location to better reflect your primary storage area (e.g., 'Head Office', 'Central Warehouse').")]
        [ProtoMember(1), Placeholder(nameof(Strings.DefaultInventoryLocation)), NoWrap] public string Name { get; set; }
        [Guide("Optionally assign a code to the default location for use in reports and quick identification.")]
        [ProtoMember(3), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }

        string ICode.Code => Code;

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.DefaultInventoryLocation;
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            return true;
        }
    }
}

using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete07
{
    [ProtoContract]
    [Guid("f6913cf9-9d2f-4f08-9edd-b29d2a363fe3")]
    internal sealed class TaxCode07 : Object
    {
        [ProtoMember(3)]
        public string Code;
        [ProtoMember(4)]
        public decimal Rate;
        [ProtoMember(5)]
        public string Notes;
    }
}

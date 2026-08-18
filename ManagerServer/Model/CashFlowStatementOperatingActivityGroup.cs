using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("25299eaa-3460-4a62-bd5d-8bc65b24375d")]
    public sealed class CashFlowStatementOperatingActivityGroup : NamedObject
    {
        [Guide("Enter the name for this operating activity group. This will appear as a category on the cash flow statement under operating activities.")]
        [ProtoMember(1)] public string Name { get; set; }

        public override string GetName()
        {
            return Name;
        }
    }
}
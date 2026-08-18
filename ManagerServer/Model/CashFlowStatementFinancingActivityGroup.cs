using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("da08116a-6fe3-47c2-805c-d5a81a03931a")]
    public sealed class CashFlowStatementFinancingActivityGroup : NamedObject
    {
        [Guide("Enter the name for this financing activity group. This will appear as a category on the cash flow statement under financing activities.")]
        [ProtoMember(1)] public string Name { get; set; }

        public override string GetName()
        {
            return Name;
        }
    }
}

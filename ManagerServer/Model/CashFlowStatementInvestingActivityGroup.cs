using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("a9cf8675-afb3-42d6-9440-f5efedef55b8")]
    public sealed class CashFlowStatementInvestingActivityGroup : NamedObject
    {
        [Guide("Enter the name for this investing activity group. This will appear as a category on the cash flow statement under investing activities.")]
        [ProtoMember(1)] public string Name { get; set; }

        public override string GetName()
        {
            return Name;
        }
    }
}

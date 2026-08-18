using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("e3ea8ce1-4fa4-43df-aec6-38ef5b574b1b")]
    public sealed class BillableExpenses : ManagerServer.Model.Object
    {
        [Guide("Check this box to enable billable expense tracking. This allows marking expenses as billable to specific customers.")]
        [ProtoMember(1)] public bool Enabled { get; set; }
    }
}

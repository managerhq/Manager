using ProtoBuf;
using System;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete80
{
    [ProtoContract]
    [Guid("c4f5eb3c-45ea-424d-aa25-ee25dad0cf80")]
    public sealed class InventoryRevaluationWorksheet : Object
    {
        [ProtoMember(1)] public DateTime Date;
    }
}
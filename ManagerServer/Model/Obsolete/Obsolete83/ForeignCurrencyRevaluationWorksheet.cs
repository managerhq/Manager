using ProtoBuf;
using System;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete83
{
    [ProtoContract]
    [Guid("4e639a4a-53d4-4796-b924-aa446a94d15f")]
    public sealed class ForeignCurrencyRevaluationWorksheet : Object
    {
        [ProtoMember(1)] public DateTime Date;
    }
}
using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete71
{
    [ProtoContract]
    [Guid("5841ea5a-fc02-4f67-83f3-9b23c5b28241")]
    public sealed class EmailTemplate : Object
    {
        [ProtoMember(1)] public string Subject;
        [ProtoMember(2)] public string Body;
    }
}

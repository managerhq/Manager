using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete90
{
    [ProtoContract]
    [Guid("72658ef1-cf0d-4d68-bcbb-219304a8180e")]
    public sealed class BankFeedProvider : Model.Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public string Endpoint;
    }
}

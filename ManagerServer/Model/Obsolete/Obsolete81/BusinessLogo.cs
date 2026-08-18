using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete81
{
    [ProtoContract]
    [Singleton]
    [Guid("096d0af9-df72-425d-aae8-d59c0497f119")]
    public sealed class BusinessLogo : Object
    {
        [ProtoMember(1)] public string ContentType;
        [ProtoMember(3)] public byte[] Content;

        [ProtoMember(2)] public Guid Obsolete_BlobID;
    }
}

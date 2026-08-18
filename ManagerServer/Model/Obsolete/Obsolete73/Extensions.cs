using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete73
{
    [Singleton]
    [ProtoContract]
    [Guid("002b86a3-554e-4a86-9395-6c6575d1d055")]
    public sealed class Extensions : Object
    {
        [ProtoMember(1)] public bool Edit;
    }
}

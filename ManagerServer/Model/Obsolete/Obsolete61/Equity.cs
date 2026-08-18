using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete61
{
    [ProtoContract]
    [Guid("97f97d83-3cc9-42e9-9c21-5ecff81fb0cb")]
    public sealed class Equity : ManagerServer.Model.Object
    {
        [ProtoMember(1)]
        public string Name;
    }
}
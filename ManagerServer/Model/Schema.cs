using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("a9a71e47-82b3-49db-8aec-898adb460a80")]
    public sealed class Schema : Object
    {
        [Guide("The current schema version number. This is automatically updated during system upgrades.")]
        [ProtoMember(1)] public int Version { get; set; }
    }
}

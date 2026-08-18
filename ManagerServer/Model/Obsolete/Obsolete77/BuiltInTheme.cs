using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete77
{
    [ProtoContract]
    [Guid("6b7eef68-b106-4c68-81de-51efdb54c0dd")]
    public sealed class BuiltInTheme : Object
    {
        [ProtoMember(1)] public bool Active;
    }
}

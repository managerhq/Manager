using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete32
{
    [ProtoContract]
    [Guid("fca69b5a-f453-4a7f-a9dc-a51c16f4e402")]
    internal sealed class ViewTemplate32 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public string Markup;
    }
}

using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete39
{
    [ProtoContract]
    [Guid("9c3968b2-d79a-4134-837a-b906a4ee0c60")]
    internal sealed class ActivationKey39 : Object
    {
        [ProtoMember(2)]
        public int Code;

        [ProtoMember(1)]
        public Guid Obsolete_Distributor;
    }
}

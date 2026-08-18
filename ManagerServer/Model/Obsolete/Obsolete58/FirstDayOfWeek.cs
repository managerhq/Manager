using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete58
{
    [ProtoContract]
    [Guid("81e2618b-2d7a-40ae-989a-a70981566692")]
    public sealed class FirstDayOfWeek : Object
    {
        [ProtoMember(1)]
        public int Value;
    }
}

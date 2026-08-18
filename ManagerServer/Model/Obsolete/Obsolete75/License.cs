using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete75
{
    [ProtoContract]
    [Singleton]
    [Guid("874bce81-e976-4323-bfcd-b2412868c34a")]
    public sealed class License : Object
    {
        [ProtoMember(6)] public string ProductKey;
        [ProtoMember(7)] public DateTime ExpityDate;
    }
}

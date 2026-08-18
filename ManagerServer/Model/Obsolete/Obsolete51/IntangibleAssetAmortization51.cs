using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete51
{
    [ProtoContract]
    [Guid("476672a0-db0c-405a-ae04-09497f23dc9c")]
    internal sealed class IntangibleAssetAmortization51 : Object
    {
        [ProtoMember(1)]
        public Guid IntangibleAsset;
        [ProtoMember(2)]
        public DateTime Date;
        [ProtoMember(3)]
        public decimal Amount;
        [ProtoMember(4)]
        public string Description;
        [ProtoMember(5)]
        public Dictionary<Guid, string> CustomFields;
        [ProtoMember(6)]
        public Guid? TrackingCode;
    }
}

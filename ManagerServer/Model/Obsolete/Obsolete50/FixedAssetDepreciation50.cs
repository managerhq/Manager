using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete50
{
    [ProtoContract]
    [Guid("2e8089f0-eda0-41c1-b3d2-2593d1c6eac2")]
    internal sealed class FixedAssetDepreciation50 : Object
    {
        [ProtoMember(1)]
        public Guid? FixedAsset;
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

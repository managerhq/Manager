using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete35
{
    [ProtoContract]
    [Guid("0a3d2c60-4bed-418f-b0ee-c8a479e62174")]
    internal sealed class ExchangeRates35 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public ExchangeRate[] Rates;

        [ProtoContract]
        public sealed class ExchangeRate
        {
            [ProtoMember(1)]
            public Guid Currency;
            [ProtoMember(2)]
            public decimal? Rate;
        }
    }
}

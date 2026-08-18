using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("ed5a19f6-12c5-45cc-b4b7-4e79f7ef50bc")]
    public sealed class Liabilities : BalanceSheetAbstractGroup
    {
        public override string GetName()
        {
            return Strings.Liabilities;
        }
    }
}
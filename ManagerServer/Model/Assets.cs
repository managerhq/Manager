using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("4c05c221-ca57-4c7c-be62-115669302ed4")]
    public sealed class Assets : BalanceSheetAbstractGroup
    {
        public override string GetName()
        {
            return Strings.Assets;
        }
    }
}

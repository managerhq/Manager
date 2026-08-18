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
    [Singleton]
    [ProtoContract]
    [Guid("0b3fe333-755b-42c0-b921-2835e39e50f0")]
    public sealed class ReverseSignReportingCategory : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.ReverseSigns;
        }
    }
}

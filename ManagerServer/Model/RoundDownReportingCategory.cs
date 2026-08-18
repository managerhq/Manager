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
    [Guid("0846abcd-dc2a-4914-9c9b-9d7130ac99d6")]
    public sealed class RoundDownReportingCategory : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.RoundDown;
        }
    }
}

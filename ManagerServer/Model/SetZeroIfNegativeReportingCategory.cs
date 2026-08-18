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
    [Guid("1a94b65c-4869-4138-acc1-49d16bbfeed6")]
    public sealed class SetZeroIfNegativeReportingCategory : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.SetZeroIfNegative;
        }
    }
}

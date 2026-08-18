using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("7d3ddc8b-49f1-4064-997a-430367e54055")]
    public sealed class ReportTransformationFromDateLastJuly : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return "Since last July";
        }
    }
}

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
    [Guid("211bb6c2-9ca7-4cda-a099-99bcde19a173")]
    public sealed class ReportTransformationTaxSales : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.Sales;
        }
    }
}

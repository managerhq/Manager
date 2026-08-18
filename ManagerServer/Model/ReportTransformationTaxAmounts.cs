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
    [Guid("094377dd-1f71-40cf-bb48-58daf961aa71")]
    public sealed class ReportTransformationTaxAmounts : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.TaxAmount;
        }
    }
}

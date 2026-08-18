using System;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("cef33379-d1b3-4172-b090-0fc24cf978da")]
    public sealed class ReportTranformationFromDate : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.ReportTransformation + " - " + Strings.FromDate;
        }
    }
}

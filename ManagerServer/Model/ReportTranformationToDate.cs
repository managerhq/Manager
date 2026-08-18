using System;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("8ba7e5e7-8f74-443a-b7ee-d8539b12e7e2")]
    public sealed class ReportTranformationToDate : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.ReportTransformation + " - " + Strings.ToDate;
        }
    }
}

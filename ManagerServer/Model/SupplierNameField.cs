using System;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("22ec22e1-8ed2-4cba-a5b9-533a1e451977")]
    public sealed class SupplierNameField : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.Supplier + " - " + Strings.Name;
        }
    }
}

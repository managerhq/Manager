using System;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("db71c44c-ec5a-4701-aa54-67ada72aff1a")]
    public sealed class EmployeeNameField : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.Employee + " - " + Strings.Name;
        }
    }
}

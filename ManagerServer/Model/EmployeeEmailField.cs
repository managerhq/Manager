using System;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("f66ab672-c1c6-4280-9439-bdb0a72b7619")]
    public sealed class EmployeeEmailField : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.Employee + " - " + Strings.EmailAddress;
        }
    }
}

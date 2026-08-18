using System;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("ce6302f8-0b02-42d8-b6b7-850063e4bbe0")]
    public sealed class BusinessDetailsNameField : NamedObject, IReportingCategory
    {
        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return Strings.BusinessDetails + " - " + Strings.Name;
        }
    }
}

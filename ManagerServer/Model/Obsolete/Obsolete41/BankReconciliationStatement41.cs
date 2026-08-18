using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete41
{
    [ProtoContract]
    [Guid("6f964580-74e7-47f8-ba95-d104545fb371")]
    internal sealed class BankReconciliationStatement41 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public Guid? BankAccount;
    }
}

using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete22
{
    [ProtoContract]
    [Guid("c862a781-56eb-4e92-ac9a-563667d0752c")]
    internal sealed class BankAccount22 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(3)]
        public string FinancialInstitution;
        [ProtoMember(4)]
        public string AccountNumber;
        [ProtoMember(5)]
        public decimal StartingBalance;
        [ProtoMember(10)]
        public Guid? Currency;
        [ProtoMember(14)]
        public decimal? CreditLimit;
        [ProtoMember(15)]
        public bool HasStartingBalance;

        [ProtoMember(6)]
        public DateTime Obsolete_StartingBalanceDate;
        [ProtoMember(7)]
        public decimal Obsolete_BankOverdraftLimit;
        [ProtoMember(8)]
        public bool Obsolete_CustomBankReconciliationDate;
        [ProtoMember(9)]
        public DateTime? Obsolete_BankReconciliationDate;
        [ProtoMember(13)]
        public EnabledDisabled Obsolete_ChequeFacility;
        [ProtoMember(11)]
        public DateTime? Obsolete_BankReconciliationDate2;
        [ProtoMember(12)]
        public EnabledDisabled Obsolete_BankReconciliationAssistant;
    }
}

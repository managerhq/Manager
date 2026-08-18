using ProtoBuf;
using System;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete83
{
    [ProtoContract]
    [Guid("4e960f93-2fd5-4846-a357-9dc9e6385406")]
    public sealed class CurrencyRevaluation : ManagerServer.Model.Object
    {
        [ProtoMember(1)] public DateTime Date;
        [ProtoMember(27)] public string Description;
        [ProtoMember(25)] public Line[] Lines;

        [ProtoMember(26)] public RealizedGain[] Obsolete_RealizedGains;

        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1)] public Guid? Account;
            [ProtoMember(2)] public Guid? BankOrCashAccount;
            [ProtoMember(3)] public Guid? Customer;
            [ProtoMember(4)] public Guid? Supplier;
            [ProtoMember(5)] public Guid? Employee;
            [ProtoMember(6)] public Guid? SpecialAccount;
            [ProtoMember(8)] public decimal UnrealizedGains;

            [ProtoMember(7)] public decimal Obsolete_GainLoss;
        }

        [ProtoContract]
        public sealed class RealizedGain
        {
            [ProtoMember(1)] public Guid? Currency;
            [ProtoMember(2)] public decimal RealizedGainLoss;
        }
    }
}

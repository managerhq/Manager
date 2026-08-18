using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete10
{
    [ProtoContract]
    [Guid("d50d33a6-f7a7-4002-9250-6a26efae9988")]
    internal sealed class SampleChartOfAccounts10 : Object
    {
        [ProtoMember(1)]
        public bool Sales;
        [ProtoMember(2)]
        public bool InterestReceived;
        [ProtoMember(3)]
        public bool Telephone;
        [ProtoMember(4)]
        public bool Electricity;
        [ProtoMember(5)]
        public bool Rent;
        [ProtoMember(6)]
        public bool AccountingFees;
        [ProtoMember(7)]
        public bool AdvertisingAndPromotion;
        [ProtoMember(8)]
        public bool ComputerEquipment;
        [ProtoMember(9)]
        public bool Donations;
        [ProtoMember(10)]
        public bool Entertainment;
        [ProtoMember(11)]
        public bool BankCharges;
        [ProtoMember(12)]
        public bool LegalFees;
        [ProtoMember(13)]
        public bool MotorVehicleExpenses;
        [ProtoMember(14)]
        public bool PrintingAndStationery;
        [ProtoMember(15)]
        public bool RepairsAndMaintenance;
    }
}

using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete66
{
    [ProtoContract]
    [Guid("699401fe-23fd-4b52-9699-b88b36fa6b26")]
    public sealed class ReceiptOrPayment : Object
    {
        [ProtoMember(19)]
        public ReceiptOrPaymentType Type;        
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(20)]
        public string Reference;
        [ProtoMember(2)]
        public Guid? BankAccount;        
        [ProtoMember(3)]
        public string Description;
        [ProtoMember(4)]
        public Obsolete.Obsolete76.TransactionLine[] Lines;
        [ProtoMember(6)]
        public string Contact;
        [ProtoMember(8)]
        public Dictionary<Guid, string> CustomFields;        
        [ProtoMember(11)]
        public DateTime? BankClearDate;
        [ProtoMember(12)]
        public BankClearStatus BankClearStatus;
        [ProtoMember(14)]
        public Guid? InventoryLocation;
        [ProtoMember(16)]
        public bool AmountsIncludeTax;
        [ProtoMember(17)]
        public bool CustomTheme;
        [ProtoMember(18)]
        public Guid? Theme;
        [ProtoMember(22)]
        public bool AutomaticReference;
        [ProtoMember(23)]
        public bool HasPaymentCustomTitle;
        [ProtoMember(24)]
        public string PaymentCustomTitle;
        [ProtoMember(25)]
        public bool HasReceiptCustomTitle;
        [ProtoMember(26)]
        public string ReceiptCustomTitle;
        [ProtoMember(28)]
        public PayerPayeeType? PayerPayeeType;
        [ProtoMember(29)]
        public Guid? Customer;
        [ProtoMember(30)]
        public Guid? Supplier;

        [ProtoMember(5)]
        public string Obsolete_Reference;
        [ProtoMember(21)]
        public bool Obsolete_CopyFromCashTransaction;        
        [ProtoMember(27)]
        public Guid? Obsolete_CustomerOrSupplier;
    }
}
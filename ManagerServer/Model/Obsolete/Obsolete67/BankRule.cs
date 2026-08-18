using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete67
{
    [ProtoContract]
    [Guid("79694e0a-65f6-47b2-a655-2007c11c3122")]
    public sealed class BankRule : Object
    {
        [ProtoMember(1)] public Guid? IfBankAccountIs;
        [ProtoMember(2)] public string AndDescriptionContains;
        [ProtoMember(13)] public PayerPayeeType PayerOrPayee;
        [ProtoMember(14)] public Guid? Customer;
        [ProtoMember(15)] public Guid? Supplier;
        [ProtoMember(16)] public string OtherContact;
        [ProtoMember(17)] public Line[] Lines;

        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1)] public Guid? Account;
            [ProtoMember(2)] public Guid? AccountsReceivableCustomer;
            [ProtoMember(3)] public Guid? BillableExpenseCustomer;
            [ProtoMember(4)] public Guid? AccountsPayableSupplier;
            [ProtoMember(5)] public Guid? CapitalAccount;
            [ProtoMember(6)] public Guid? SubAccount;
            [ProtoMember(7)] public Guid? Employee;
            [ProtoMember(10)] public Guid? SpecialAccount;
            [ProtoMember(11)] public Guid? FixedAsset;
            [ProtoMember(12)] public Guid? IntangibleAsset;
            [ProtoMember(13)] public Guid? ExpenseClaimPayer;
            [ProtoMember(14)] public Guid? TaxCode;
            [ProtoMember(15)] public Guid? TrackingCode;
        }

        [ProtoMember(12)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines;
        [ProtoMember(4)] public string Obsolete_Name;
        [ProtoMember(6)] public Guid? Obsolete_Customer;
        [ProtoMember(7)] public Guid? Obsolete_Supplier;
        [ProtoMember(8)] public Guid? Obsolete_Employee;
        [ProtoMember(9)] public Guid? Obsolete_CapitalAccount;
        [ProtoMember(3)] public Guid? Obsolete_GeneralLedgerAccount;
        [ProtoMember(5)] public Guid? Obsolete_TaxCode;
        [ProtoMember(10)] public Guid? Obsolete_CapitalSubAccount;
        [ProtoMember(11)] public Guid? Obsolete_TrackingCode;
    }
}

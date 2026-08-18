using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete06
{
    [ProtoContract]
    [Guid("e1cf015a-89af-412f-80a2-c9b98d969cd1")]
    internal sealed class GeneralSettings06 : Object
    {
        [ProtoMember(1)]
        public int? Country;
        [ProtoMember(2)]
        public bool? Country_Australia_GST;
        [ProtoMember(3)]
        public bool? Country_UnitedKingdom_VAT;
        [ProtoMember(4)]
        public bool? BankAccounts;
        [ProtoMember(5)]
        public bool? SalesInvoices;
        [ProtoMember(6)]
        public bool? AccountsPayable;
        [ProtoMember(7)]
        public bool? TaxCodes;
        [ProtoMember(9)]
        public bool? OutOfPocketExpenses;
        [ProtoMember(10)]
        public bool? SalesQuotes;
        [ProtoMember(12)]
        public bool? Country_NewZealand_GST;
        [ProtoMember(13)]
        public bool? SalesInvoiceItems;
        [ProtoMember(16)]
        public bool? Country_SouthAfrica_VAT;
        [ProtoMember(17)]
        public bool? Country_Philippines_VAT;
        [ProtoMember(18)]
        public bool? CashAccounts;
        [ProtoMember(19)]
        public bool? Country_India_TaxDeductedAtSource;

        [ProtoMember(11)]
        public DateTime? Obsolete_SummaryStartDate;
        [ProtoMember(15)]
        public bool? Obsolete_AccountsReceivable;
    }
}

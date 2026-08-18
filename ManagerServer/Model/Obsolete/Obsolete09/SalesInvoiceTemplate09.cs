using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete09
{
    [ProtoContract]
    [Guid("0b96900e-552d-4d14-a070-9086e44f188d")]
    internal sealed class SalesInvoiceTemplate09 : Object
    {
        [ProtoMember(1)]
        public string BusinessName;
        [ProtoMember(2)]
        public string BusinessContactInformation;
        [ProtoMember(3)]
        public string BusinessIdentifier;
        [ProtoMember(4)]
        public string TermsAndPaymentAdvice;
        [ProtoMember(6)]
        public bool? EnableVisaOnPaymentAdviceCutAway;
        [ProtoMember(7)]
        public bool? EnableMastercardOnPaymentAdviceCutAway;
        [ProtoMember(8)]
        public bool? EnableAmericanExpressOnPaymentAdviceCutAway;
        [ProtoMember(9)]
        public bool? EnableDinersOnPaymentAdviceCutAway;
        [ProtoMember(10)]
        public bool? EnablePaymentAdviceCutAway;
        [ProtoMember(11)]
        public bool DisplayLogo;
        [ProtoMember(12)]
        public string StandardTermsOnQuote;
        [ProtoMember(17)]
        public int? CustomerAddressHorizontalPadding;
        [ProtoMember(18)]
        public int? CustomerAddressVerticalPadding;
        [ProtoMember(19)]
        public string CurrencySymbolPrefix;
        [ProtoMember(20)]
        public string CurrencySymbolSuffix;
        [ProtoMember(21)]
        public string TaxIdentifier;
        [ProtoMember(22)]
        public string SalesInvoiceTitle;
        [ProtoMember(23)]
        public bool AmountsIncludeTax;
        [ProtoMember(24)]
        public string SalesInvoiceNumberPrefix;

        [ProtoMember(13)]
        public string Obsolete_Australia_ABN_Number;
        [ProtoMember(14)]
        public string Obsolete_NewZealand_GST_Number;
        [ProtoMember(15)]
        public string Obsolete_SouthAfrica_VAT_Number;
        [ProtoMember(16)]
        public string Obsolete_Philippines_TIN_Number;
    }
}

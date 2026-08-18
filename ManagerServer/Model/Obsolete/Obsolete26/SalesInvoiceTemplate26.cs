using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete26
{
    [ProtoContract]
    [Guid("55c81ff0-2892-41fb-bff8-3fef6debba85")]
    internal sealed class SalesInvoiceTemplate26 : Object
    {
        [ProtoMember(1)]
        public string TermsAndPaymentAdvice;
        [ProtoMember(7)]
        public string Title;
        [ProtoMember(9)]
        public string ReferenceNumberPrefix;

        [ProtoMember(10)]
        public string Obsolete_Notes;
        [ProtoMember(8)]
        public bool Obsolete_AmountsIncludeTax;
        [ProtoMember(2)]
        public bool? Obsolete_EnablePaymentAdviceCutAway;
        [ProtoMember(3)]
        public bool? Obsolete_EnableVisaOnPaymentAdviceCutAway;
        [ProtoMember(4)]
        public bool? Obsolete_EnableMastercardOnPaymentAdviceCutAway;
        [ProtoMember(5)]
        public bool? Obsolete_EnableAmericanExpressOnPaymentAdviceCutAway;
        [ProtoMember(6)]
        public bool? Obsolete_EnableDinersOnPaymentAdviceCutAway;
    }
}

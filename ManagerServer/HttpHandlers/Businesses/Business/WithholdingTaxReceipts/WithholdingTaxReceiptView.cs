using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    [Guide("This view displays the details of a *withholding tax receipt* issued to a customer.")]
    [Guide("*Withholding tax receipts* are official documents that confirm tax has been withheld from payments made to customers. These receipts are typically required for the customer's tax compliance and record-keeping purposes.")]
    [LinkGuide("For more information, see:", typeof(WithholdingTaxReceiptForm))]
    internal sealed class WithholdingTaxReceiptView : TransactionView<ManagerServer.Model.WithholdingTaxReceipt>
    {
        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new WithholdingTaxReceiptTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
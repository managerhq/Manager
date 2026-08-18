using System;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System.Collections.Generic;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [Title(nameof(Strings.CreditNote))]
    [Guide("This screen displays a credit note in its final format, ready for printing or sending to customers.")]
    [Guide("A credit note is a document issued to customers to acknowledge that they are owed money, typically due to returned goods, pricing errors, or other adjustments to their account.")]
    [Header("Available Actions")]
    [Guide("Use the action buttons at the top of the screen to manage this credit note:")]
    [Guide("• `Print` - Generate a PDF version of the credit note for printing or saving")]
    [Guide("• `Email` - Send the credit note directly to the customer's email address")]
    [Guide("• `Edit` - Make changes to the credit note details, amounts, or line items")]
    [Guide("• `Copy to` - Create new transactions based on this credit note")]
    [Header("Document Information")]
    [Guide("The credit note displays key information including the issue date, reference number, customer details, and billing address.")]
    [Guide("If the credit note is linked to a specific sales invoice, that invoice reference will be shown for easy tracking.")]
    [Guide("Line items show the products or services being credited, along with quantities, unit prices, and tax calculations.")]
    [LinkGuide("To learn how to create or modify credit notes, see:", typeof(CreditNoteForm))]
    internal sealed class CreditNoteView : TransactionView<ManagerServer.Model.CreditNote>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForCreditNote>();
        }

        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction)];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new CreditNoteTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
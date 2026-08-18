using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DebitNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DebitNote))]
    [Guide("The `Debit Note` view displays detailed information about a debit note issued to a supplier.")]
    [Guide("A debit note is a document sent to a supplier to inform them that their account has been debited, typically for returned goods, price adjustments, or billing errors.")]
    [Guide("From this view, you can edit the debit note details, print a copy for your records, or email it directly to the supplier.")]
    [LinkGuide("For more information, see:", typeof(DebitNoteForm))]
    internal sealed class DebitNoteView : TransactionView<ManagerServer.Model.DebitNote>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForDebitNote>();
        }

        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction)];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new DebitNoteTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
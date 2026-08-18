using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DebitNotes
{
    [ProtoContract]
    [Guid("A7775F5E-9252-4716-8507-94E66195DA7E")]
    [Title(nameof(Strings.DebitNote), nameof(Strings.Lines))]
    [Guide("This screen displays all line items from debit notes in your business.")]
    [Guide("A debit note is a document sent to a supplier indicating that you are debiting their account, typically for goods returned or price adjustments.")]
    [Guide("This report provides a comprehensive view of all individual line items from all debit notes, allowing you to analyze debited products and services in detail.")]
    [Guide("Use this screen to review line-level details across multiple debit notes, track returned items, monitor price adjustments, and analyze debit patterns by supplier, item, or account.")]
    [Header("Understanding the Information")]
    [Guide("Each row in the table represents a single line item from a debit note.")]
    [Guide("The table consolidates line items from all debit notes, making it easy to see all debited items in one place.")]
    [Guide("You can click on any line to view or edit the complete debit note that contains it.")]
    [Columns]
    internal sealed class DebitNoteLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.DebitNote.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<DebitNote>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new DebitNoteForm() { Business = Business, Key = x.DebitNote.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new DebitNoteView() { Business = Business, Key = x.DebitNote.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("9878BCE6-D14D-496D-8D9F-4B2E9568776A")]
        public DateTime[] GetIssueDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.DebitNote.IssueDate).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guid("3A205D7A-6FBD-45C9-B81D-7EBEE5014AEB")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.DebitNote.Reference).ToArray();
        }

        [Default]
        [Guid("5A23A82D-A6BC-4B1D-B0FA-9E581574CB6A")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("0EFFB3B3-19A8-4FF0-B77B-850151D41DD2")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.DebitNote.Description).ToArray();
        }

        [Guid("4A29556D-36DF-4B09-89B9-6EEAF638D27C")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("E18C35CF-8D01-4B5E-B590-100CE6763C44")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("29C839DA-69A3-4EFB-9986-A2DABF55FB5B")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Guid("DC51F9B5-E7C5-4BCE-B00C-C21FF92559CD")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value * -1m : default(decimal?)).ToArray();
        }

        [Guid("62670D8B-1ADC-4D2C-8248-F3B7D5B7C22A")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("AD8D1264-9DE0-46C6-A0F6-CD885B3EEEA3")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("A373D64A-779C-484F-BCB2-4EFCAE6E9542")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("5C17F6CB-15C6-4DD1-9770-46BB6E6C3084")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("4CBCE143-A22D-4305-BEF9-E557A9730622")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Sum, Right]
        [Guid("D2FACD11-4222-4B13-9BA2-150FF4DCA469")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount*-1m, x.TransactionCurrency)).ToArray();
        }
    }
}
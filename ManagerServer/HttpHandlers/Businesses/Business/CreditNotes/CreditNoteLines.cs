using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [Guid("A684FA5A-69D1-436F-A534-FB1EA32C8C42")]
    [Title(nameof(Strings.CreditNote), nameof(Strings.Lines))]
    [Guide("This screen displays individual line items from all credit notes in one consolidated view, allowing you to analyze credited products, services, and amounts across your entire credit note history.")]
    [Guide("Each row represents a single line item from a credit note, showing details such as the item, quantity, unit price, and total amount credited.")]
    [Guide("Use this view to track which items are most frequently credited, identify patterns in returns or adjustments, and analyze credit note trends by customer, date range, or other criteria.")]
    [Columns]
    internal sealed class CreditNoteLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.CreditNote.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<CreditNote>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new CreditNoteForm() { Business = Business, Key = x.CreditNote.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new CreditNoteView() { Business = Business, Key = x.CreditNote.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("56E1698C-E937-4965-96AF-E9F421840459")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("32FCA433-35C7-42E4-A1A6-1F5F789345D1")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.CreditNote.Reference).ToArray();
        }

        [Default]
        [Guid("CEFCFD3C-E154-4840-9E39-49B1342A8C20")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("66EACEDC-FB5D-4580-A18F-0CAFE7C99E00")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.CreditNote.Description).ToArray();
        }

        [Guid("F16DABAA-4B57-46F1-8DE7-EAEDF646A90E")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("6F2DBBD6-8981-467A-9A17-D61DB4FFC2D8")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("45946FA3-D7F0-40E6-8329-18B4D24CCF83")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Guid("0B9B5F07-CA5F-44A1-AA6E-FE9F43EB9EFD")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Guid("FB523085-D0A3-4800-A2C8-4A22A5732889")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("053956E7-CA77-4F4E-8B56-584A3B732B20")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("02DC1686-C806-40E0-94AC-DC5435478317")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("1EFA3F12-AB46-4783-A644-C18AA015B10C")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("155DFAEA-85D6-4B06-9334-F8B0C2E422A4")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("8FEA7076-CA8D-4C71-8D5B-5A196A0F5E5B")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount, x.TransactionCurrency)).ToArray();
        }
    }
}

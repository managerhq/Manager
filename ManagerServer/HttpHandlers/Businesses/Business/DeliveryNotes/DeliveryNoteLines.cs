using ManagerServer.Model;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    [Guid("10FDA91F-F706-4B4E-81B8-B98F7B7D71F4")]
    [Title(nameof(Strings.DeliveryNote), nameof(Strings.Lines))]
    [Guide("This screen displays all line items from delivery notes in a comprehensive table format.")]
    [Guide("It provides a detailed view of all products and services delivered across all delivery notes, allowing you to analyze deliveries at the line-item level.")]
    [Guide("Use this screen to review delivery patterns, track specific items across multiple delivery notes, or verify quantities delivered to customers.")]
    [Guide("Each row represents a single line item from a delivery note, showing the item details and quantity delivered.")]
    [Guide("The table displays the following information:")]
    [Columns]
    internal sealed class DeliveryNoteLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.DeliveryNote.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<DeliveryNote>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new DeliveryNoteForm() { Business = Business, Key = x.DeliveryNote.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new DeliveryNoteView() { Business = Business, Key = x.DeliveryNote.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("67C0CA00-11CC-4D2C-9AAD-A3930378F73A")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("5970697A-62DD-4388-A871-658F8CB481B7")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.DeliveryNote.Reference).ToArray();
        }

        [Guid("A5EACE20-9A53-49CC-A160-655F6A9DE7CB")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("25599A3B-A76A-406B-A4C3-D51182AD2A73")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.DeliveryNote.Description).ToArray();
        }

        [Default]
        [Guid("D5D0783D-105D-4F8A-A960-9707EA35B5E8")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Guid("39686E98-C731-4A02-9126-0DB57A736EEC")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("6F23B62B-DF83-487D-B577-23FC99C26A95")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value : default(decimal?)).ToArray();
        }
    }
}

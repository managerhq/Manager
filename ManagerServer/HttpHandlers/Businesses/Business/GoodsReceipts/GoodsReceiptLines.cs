using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    [Guid("142F49D0-25B3-4165-8C7C-F1E6A92A56DE")]
    [Title(nameof(Strings.GoodsReceipt), nameof(Strings.Lines))]
    [Guide("The **Goods Receipt Lines** screen displays all individual line items from all *goods receipts* in your business.")]
    [Guide("This consolidated view allows you to see all products and inventory items that have been received, regardless of which goods receipt they belong to.")]
    [Header("Overview")]
    [Guide("Each row in the table represents a single line item from a goods receipt, showing the product details, quantity received, and associated information.")]
    [Guide("You can use this screen to quickly review all incoming inventory, track specific items across multiple receipts, or verify quantities received from suppliers.")]
    [Header("Working with the Table")]
    [Guide("The table displays comprehensive information about each goods receipt line item, including the date received, reference number, supplier details, and item quantities.")]
    [Guide("Click on any row to view or edit the complete goods receipt that contains that line item.")]
    [Columns]
    internal sealed class GoodsReceiptLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.GoodsReceipt.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<GoodsReceipt>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new GoodsReceiptForm() { Business = Business, Key = x.GoodsReceipt.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new GoodsReceiptView() { Business = Business, Key = x.GoodsReceipt.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("FE111A1A-4B2F-415C-BF14-F4E7BECAA398")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("9FD968D3-EAAA-4320-8EA3-B601EC6CBA2A")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.GoodsReceipt.Reference).ToArray();
        }

        [Guid("0F786CAC-CF89-4529-BE68-4571D01437EE")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("7AB94475-2C67-4891-8C5C-34490D349492")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.GoodsReceipt.Description).ToArray();
        }

        [Default]
        [Guid("C36AA387-22DC-4283-B3C2-57836D70D6F8")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Guid("0EBDD914-6426-462A-B7FD-49BF59E4E9E2")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("0CAAB4B7-FCB9-47D8-BCB9-09D850544F91")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value : default(decimal?)).ToArray();
        }
    }
}

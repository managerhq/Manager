using ManagerServer.Model;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [Guid("FA03BB66-46EA-4BAC-BE41-338935E77B8F")]
    [Title(nameof(Strings.PurchaseOrders), nameof(Strings.Lines))]
    [Guide("The **Purchase Orders - Lines** screen allows you to view individual lines from all purchase orders in one place.")]
    [Guide("This consolidated view helps you find specific purchase orders based on their line items, track ordered quantities across multiple orders, and analyze purchase patterns.")]
    [Header("Accessing Purchase Order Lines")]
    [Guide("To access the **Purchase Orders - Lines** screen, navigate to the **Purchase Orders** tab.")]
    [TabScreenshot("fa-shopping-cart", nameof(Strings.PurchaseOrders))]
    [Guide("Click the **Purchase Orders - Lines** button located in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(name: "PurchaseOrders-Lines")]
    [Header("Customizing Your View")]
    [Guide("You can customize which columns appear in the table by clicking the **Edit Columns** button. This allows you to show or hide information such as *item codes*, *quantities*, *unit prices*, *tax amounts*, and more.")]
    [LinkGuide("Learn more about customizing columns:", typeof(NakedObjectsWithEditColumns<>))]
    [Guide("Use **Advanced Queries** to filter purchase order lines by specific criteria, sort them in different ways, or create summaries for reporting purposes.")]
    [LinkGuide("Learn more about advanced queries:", typeof(NakedObjectsWithAdvancedQueries))]
    internal sealed class PurchaseOrderLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.PurchaseOrder.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<PurchaseOrder>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseOrderForm() { Business = Business, Key = x.PurchaseOrderAsTransaction.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseOrderView() { Business = Business, Key = x.PurchaseOrderAsTransaction.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("13BFB077-65F3-422B-B52D-18090698C1E5")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("F0885375-8274-4F75-85F0-6A54DB1524A5")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseOrderAsTransaction.Reference).ToArray();
        }

        [Default]
        [Guid("C6D86297-F960-4CA3-BDD0-53D8C3EAED8A")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("D8991367-2BFA-44A9-AC9B-30AFB6D4227D")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseOrderAsTransaction.Description).ToArray();
        }

        [Guid("658DB069-0E54-447C-A3D8-2434B1C06EAE")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Guid("99705160-676E-4C13-85DA-D647E7CC7408")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Guid("98A0D249-7B60-431E-92B4-FD35A55C8073")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value : default(decimal?)).ToArray();
        }

        [Default]
        [Guid("97EA1035-2401-4680-B6E9-DD7DB2A4F87B")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("1AEC2FD9-3076-476F-A7A8-00352A978D73")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("E348E78B-8EFC-4D3C-AF4A-007D8FFA1C0C")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("A6B2FF6B-B2E5-4E49-A2EF-9037B116EBE0")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Guid("9AF6E4D0-0047-4906-BB93-F8EDC452A2A2")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("59128939-D27E-4EB0-A6CC-01E626D626AD")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount, x.TransactionCurrency)).ToArray();
        }
    }
}

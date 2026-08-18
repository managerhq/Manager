using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryCostingCalculationWorksheet), nameof(Strings.TotalCost), nameof(Strings.Transactions))]
    [Guide("Shows total cost transactions for inventory items using the selected valuation method.")]
    [Guide("Displays a running total of inventory costs based on receipts and deliveries.")]
    [Columns]
    internal sealed class InventoryCostingCalculationWorksheetTotalCostTransactions : NakedObjectsWithSimpleSearch
    {
        [ProtoMember(1)] public DateTime Date;
        [ProtoMember(2)] public Guid InventoryItem;

        protected override void InnerGet4(Context context)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

            var rows = new List<GeneralLedgerTransaction>();

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.Date <= Date)
                .OrderByDescending(x => x.Date)
                .ToArray();

            var qty = transactions.Sum(x => x.Qty ?? 0m);

            if (qty > 0m)
            {
                rows.AddRange(transactions);
            }

            context.Set<Array>(rows.ToArray());

            base.InnerGet4(context);
        }        

        [Icon("fa-edit")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction, referrer)).ToArray();
        }

        [Icon("fa-eye")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction, referrer)).ToArray();
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("798eba34-7ae8-4e50-9598-b26988801826")]
        [Guide("Shows the transaction date.")]
        public DateTime[] GetDate(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("361c521b-9268-49b4-b85b-24583552800e")]
        [Guide("Shows the type of transaction.")]
        public string[] GetTransaction(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetTransactionName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("ccdfe0da-6d2b-4af4-953d-e4f9013e5f7b")]
        public string[] GetCustomer(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("17f1a848-038d-4486-97b7-ff66f0b06bbc")]
        public string[] GetSupplier(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("8d3ac59b-49c0-435f-a423-9611c14458ff")]
        public NamedObject[] GetInventoryItem(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("753e39d6-227a-4bbf-ba44-f25e9ee5a9f3")]
        public string[] GetDescription(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, Sum]
        [Guid("2be7a7a6-a8cb-417d-befd-924b196e14a6")]
        [Guide("Shows the quantity of inventory items.")]
        public decimal?[] GetQty(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, Sum, RunningTotal2]
        [Guid("b1f60b0c-79bf-401c-b950-2b47f05a5b8a")]
        [Guide("Shows the running total cost of inventory.")]
        public Tuple<decimal, Currency>[] GetTotalCost(GeneralLedgerTransaction[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, Currency>(x.BaseAmount, baseCurrency)).ToArray();
        }
    }
}
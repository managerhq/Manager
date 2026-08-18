using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Model;
using ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal abstract class TransactionCosts : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        [InheritedProtoMember(1)] public Guid Transaction;
        [InheritedProtoMember(2)] public bool ReverseSign;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var transaction = database.SingleOrDefault(Transaction) as ManagerServer.Model.Transaction;

            if (transaction != null)
            {
                var rows = transaction.GetGeneralLedgerTransactions(database)
                    .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                    .Where(x => x.IsCostOfGoodsSold)
                    .ToArray();
                context.Set<Array>(rows);
            }
            else
            {
                context.Set<Array>(new GeneralLedgerTransaction[0]);
            }

            base.InnerGet4(context);
        }

        protected override void OnAfterHeader(Context context)
        {
            /*
            var unitCostColumn = context.Get<Column[]>().SingleOrDefault(x => x.Key == new Guid("5df4921e-d915-4856-802c-7498353ceeb3"));
            if (unitCostColumn != null)
            {
                var database = Manager.ApplicationData.Businesses.Get(FileID);
                var transaction = database.SingleOrDefault(Transaction) as Manager.Model.Transaction;
                var lockDate = database.Single<LockDate>();

                if (transaction.GetGeneralLedgerTransactions(database).Any())
                {
                    if (lockDate.IsLocked(transaction.GetGeneralLedgerTransactions(database).Min(x => x.Date)))
                    {
                        unitCostColumn.Action = new Tuple<string, HttpHandler, bool>(Strings.Recalculate, null, false);
                    }
                    else
                    {
                        unitCostColumn.Action = new Tuple<string, HttpHandler, bool>(Strings.Recalculate, new Settings.InventoryUnitCosts.RecalculateInventoryUnitCost() { FileID = FileID, Transaction = Transaction }, true);
                    }
                }
            }
            */

            base.OnAfterHeader(context);
        }

        [Default]
        [HideColumnIfAllEmpty]
        [Guid("ab4fa2e6-b509-4b58-949c-ee4c18dd1328")]
        public string[] GetInventoryKit(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryKit?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("96594a97-bf34-4041-a64e-d2ec29ebb91c")]
        public string[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("c92883ce-b0f5-4143-9be5-75ecc528a39a")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var multiplier = ReverseSign ? -1m : 1m;
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value * multiplier : default(decimal?)).ToArray();
        }

        [Default]
        [Right]
        [Guid("5df4921e-d915-4856-802c-7498353ceeb3")]
        public Tuple<decimal, BusinessTemplate>[] GetUnitCost(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => GetHttpHandlerForUnitCost(baseCurrency, x.InventoryUnitCost, x.InventoryItem, x.Date, referrer)).ToArray();
        }

        private Tuple<decimal, BusinessTemplate> GetHttpHandlerForUnitCost(BaseCurrency baseCurrency, InventoryUnitCost inventoryUnitCost, InventoryItem inventoryItem, DateTime date, string referrer)
        {
            if (inventoryUnitCost == null) return null;
            if (inventoryUnitCost.Date == date) return new Tuple<decimal, BusinessTemplate>(baseCurrency.Round(inventoryUnitCost.UnitCost), new InventoryUnitCostForm() { Key = inventoryUnitCost.Key, Business = Business, Referrer = referrer });
            return new Tuple<decimal, BusinessTemplate>(baseCurrency.Round(inventoryUnitCost.UnitCost), new InventoryUnitCostForm() { Date = date, InventoryItem = inventoryItem.Key, UnitCost = inventoryUnitCost.UnitCost, Business = Business, Referrer = referrer });
        }

        [Default]
        [Right, Sum, Bold]
        [Guid("7d4a3dc3-ddde-46cd-837e-642fd262b94c")]
        public decimal[] GetTotalCost(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var multiplier = ReverseSign ? -1m : 1m;
            return rows.Select(x => x.BaseAmount * multiplier).ToArray();
        }
    }
}

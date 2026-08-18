using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInventoryItems : BaseGeneralLedgerTransactionsForInterAccountTransferTransactions
    {
        [InheritedProtoMember(365)] public Guid? InventoryItemQty;
        [InheritedProtoMember(366)] public Guid? InventoryItemCost;

        protected override void OnAfterHeader(Context context)
        {
            var unitCostColumn = context.Get<Column[]>().SingleOrDefault(x => x.Key == new Guid("67d2d1e0-125b-40ef-a173-551d8c57b910"));
            if (unitCostColumn != null)
            {
                unitCostColumn.Action = new Tuple<string, HttpHandler, bool>(Strings.Recalculate, new Settings.InventoryUnitCosts.InventoryCostCorrection() { Business = Business, ToDate = To, Referrer = this.ToUrl() }, false);
            }

            base.OnAfterHeader(context);
        }

        protected override void InnerGet4(Context context)
        {
            if (!InventoryItemQty.HasValue && !InventoryItemCost.HasValue)
            {
                var balanceSheetInventoryItemsAccount = ApplicationData.Businesses.Get(Business).Single<BalanceSheetInventoryOnHandAccount>();
                var controlAccountForInventoryItems = ApplicationData.Businesses.Get(Business).SingleOrDefault<ControlAccountForInventoryItems>(GeneralLedgerAccount);

                if (controlAccountForInventoryItems != null || GeneralLedgerAccount == balanceSheetInventoryItemsAccount.Key)
                {
                    var accountBalances = GetGeneralLedgerTransactions()
                        .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                        .GroupBy(x => x.InventoryItem)
                        .Select(x => new { InventoryItem = x.Key, Balance = x.Sum(y => y.BaseAmount), Qty = x.Sum(y => y.Qty ?? 0m) })
                        .OrderByDescending(x => Math.Abs(x.Balance))
                        .ThenBy(x => x.InventoryItem.IsInactive())
                        .ThenBy(x => x.InventoryItem.GetCodeAndName())
                        .Select(x => new InventoryItemBalance()
                        {
                            InventoryItem = x.InventoryItem,
                            Balance = x.Balance,
                            Qty = x.Qty,
                            IsInactive = x.InventoryItem.IsInactive()
                        })
                        .ToArray();

                    context.Set<Array>(accountBalances);                    
                }
            }

            base.InnerGet4(context);
        }

        public sealed class InventoryItemBalance : IsInactive
        {
            public InventoryItem InventoryItem;
            public decimal Qty;
            public decimal Balance;
            public bool IsInactive;

            bool IsInactive.IsInactive => IsInactive;
        }

        [Default]
        [Guid("1a009870-c677-4198-905a-c6819f694bcb")]
        public NamedObject[] GetName(InventoryItemBalance[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default, Right]
        [Guid("8e714c47-9c6e-4b7d-9bbf-3dd70be4d6cd")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyOwned(InventoryItemBalance[] rows)
        {
            var referrer = ToUrl();

            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.Qty.TrimTrailingZeroes(),
                GetHttpHandlerWithInventoryItems(x.InventoryItem.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate GetHttpHandlerWithInventoryItems(Guid inventoryItem, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsForSubaccount;
            businessTemplate.InventoryItemQty = inventoryItem;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            businessTemplate.Term = null;
            businessTemplate.Skip = 0;
            return businessTemplate;
        }

        /*
        [Default, Right, HideColumnIfAllEmpty]
        [Guid("2a2f3b86-dc7d-48f9-aae4-84b84370296f")]
        public Tuple<decimal?, Currency, BusinessTemplate>[] GetUnitCost(InventoryItemBalance[] rows)
        {
            var database = Manager.ApplicationData.Businesses.Get(Business);
            var referrer = ToUrl();
            return GetAverageCosts(GetRoot().To, Business, rows.ToArray(), referrer);
        }
        */

        [Default, Right, Sum, Bold]
        [Guid("67d2d1e0-125b-40ef-a173-551d8c57b910")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetTotalCost(InventoryItemBalance[] rows)
        {
            var referrer = ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(x.Balance, baseCurrency, GetHttpHandlerWithInventoryItems2(x.InventoryItem.Key, referrer))).ToArray();
        }

        private BusinessTemplate GetHttpHandlerWithInventoryItems2(Guid inventoryItem, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsForSubaccount;
            businessTemplate.InventoryItemCost = inventoryItem;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            businessTemplate.Skip = 0;
            return businessTemplate;
        }

        /*
        public static Tuple<decimal?, Currency, BusinessTemplate>[] GetAverageCosts(DateTime date, string fileId, InventoryItemBalance[] rows, string referrer)
        {
            var database = Manager.ApplicationData.Businesses.Get(fileId);
            var baseCurrency = database.Single<BaseCurrency>();

            var inventoryStandardCosts = database.OfType<InventoryUnitCost>()
                .Where(x => x.Date <= date)
                .Where(x => x.InventoryItem.HasValue)
                .Where(x => x.UnitCost >= 0m)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.InventoryItem.Value)
                .ToDictionary(x => x.Key, x => x.First());

            var output = new Tuple<decimal?, Currency, BusinessTemplate>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] == null) continue;

                var row = rows[i];

                var httpHandler = new Settings.InventoryUnitCosts.InventoryUnitCostForm()
                {
                    FileID = fileId,
                    Referrer = referrer,
                    InventoryItem = row.InventoryItem.Key,
                    Date = date
                };

                if (row.Qty <= 0m)
                {
                    // No question mark if quantity negative or zero
                }
                else if (inventoryStandardCosts.TryGetValue(row.InventoryItem.Key, out InventoryUnitCost inventoryRevaluation))
                {
                    if (inventoryRevaluation.Date == date)
                    {
                        httpHandler.Key = inventoryRevaluation.Key;
                    }
                    else
                    {
                        httpHandler.UnitCost = inventoryRevaluation.UnitCost;
                    }

                    output[i] = new Tuple<decimal?, Currency, BusinessTemplate>(inventoryRevaluation.UnitCost, baseCurrency, httpHandler);
                }
                else
                {
                    output[i] = new Tuple<decimal?, Currency, BusinessTemplate>(null, baseCurrency, httpHandler);
                }
            }

            return output;
        }
        */
    }
}
using ManagerServer.Query.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer
{
    public sealed class GeneralLedgerAggregations
    {
        private readonly GeneralLedgerAggregations innerAggregations;

        private readonly Dictionary<Guid, FenwickTree> balanceSheetAccounts = [];
        private readonly Dictionary<Guid, FenwickTree> profitAndLossAccounts = [];
        private readonly Dictionary<Guid, FenwickTree> employeeBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> employeeCurrencyAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> bankOrCashAccountBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> bankOrCashAccountCurrencyAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> fixedAssetBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> depreciationBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> intangibleAssetBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> amortizationBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> inventoryItemBaseAmounts = [];
        private readonly Dictionary<Guid, FenwickTree> inventoryItemQtyOwned = [];

        public GeneralLedgerAggregations(GeneralLedgerAggregations innerAggregations = null)
        {
            this.innerAggregations = innerAggregations;
        }

        public void Update(IEnumerable<GeneralLedgerTransaction> generalLedgerTransactions, bool reverseSign)
        {
            if (generalLedgerTransactions == null) return;

            var sortedTransactions = generalLedgerTransactions.Where(x => x.Transaction == null || x.Transaction.IsGeneralLedgerTransaction()).OrderBy(x => x.Date).ToArray();

            foreach (var e in sortedTransactions.Where(x => x.ProfitAndLossAccount != null).GroupBy(x => x.ProfitAndLossAccount.Key))
            {
                UpdateBaseAmounts(profitAndLossAccounts, e.Key, e, reverseSign);
            }

            var sortedTransactions2 = sortedTransactions.ToLookup(x => x.BalanceSheetAccount);

            foreach (var e in sortedTransactions2)
            {
                UpdateBaseAmounts(balanceSheetAccounts, e.Key.Key, e, reverseSign);
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsCashAtBank))
            {
                foreach (var e2 in e.GroupBy(x => x.BankAccount.Key))
                {
                    UpdateBaseAmounts(bankOrCashAccountBaseAmounts, e2.Key, e2, reverseSign);
                    UpdateCurrencyAmounts(bankOrCashAccountCurrencyAmounts, e2.Key, e2, reverseSign);
                }
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsEmployeeClearingAccount))
            {
                foreach (var e2 in e.GroupBy(x => x.Employee.Key))
                {
                    UpdateBaseAmounts(employeeBaseAmounts, e2.Key, e2, reverseSign);
                    UpdateCurrencyAmounts(employeeCurrencyAmounts, e2.Key, e2, reverseSign);
                }
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsControlAccountForFixedAssets))
            {
                foreach (var e2 in e.GroupBy(x => x.FixedAsset.Key))
                {
                    UpdateBaseAmounts(fixedAssetBaseAmounts, e2.Key, e2, reverseSign);
                }
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsControlAccountForFixedAssetsAccumulatedDepreciation))
            {
                foreach (var e2 in e.GroupBy(x => x.FixedAsset.Key))
                {
                    UpdateBaseAmounts(depreciationBaseAmounts, e2.Key, e2, reverseSign);
                }
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsControlAccountForIntangibleAssets))
            {
                foreach (var e2 in e.GroupBy(x => x.IntangibleAsset.Key))
                {
                    UpdateBaseAmounts(intangibleAssetBaseAmounts, e2.Key, e2, reverseSign);
                }
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsControlAccountForIntangibleAssetsAccumulatedAmortization))
            {
                foreach (var e2 in e.GroupBy(x => x.IntangibleAsset.Key))
                {
                    UpdateBaseAmounts(amortizationBaseAmounts, e2.Key, e2, reverseSign);
                }
            }

            foreach (var e in sortedTransactions2.Where(x => x.Key.IsInventoryOnHand))
            {
                foreach (var e2 in e.GroupBy(x => x.InventoryItem.Key))
                {
                    UpdateBaseAmounts(inventoryItemBaseAmounts, e2.Key, e2, reverseSign);
                    UpdateQuantities(inventoryItemQtyOwned, e2.Key, e2, reverseSign);
                }
            }
        }

        public decimal GetBalanceSheetAccountBalance(Guid account, DateTime date)
        {
            return (balanceSheetAccounts.GetValueOrDefault(account)?.PrefixSum(date) ?? 0m)
                .SafeAdd(innerAggregations?.GetBalanceSheetAccountBalance(account, date) ?? 0m);
        }

        public decimal GetProfitAndLossAccountAmount(Guid account, DateTime fromDate, DateTime toDate)
        {
            return (profitAndLossAccounts.GetValueOrDefault(account)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetProfitAndLossAccountAmount(account, fromDate, toDate) ?? 0m);
        }

        public decimal GetBankOrCashAccountBaseAmount(Guid bankOrCashAccount, DateTime fromDate, DateTime toDate)
        {
            return (bankOrCashAccountBaseAmounts.GetValueOrDefault(bankOrCashAccount)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetBankOrCashAccountBaseAmount(bankOrCashAccount, fromDate, toDate) ?? 0m);
        }

        public decimal GetBankOrCashAccountCurrencyAmount(Guid bankOrCashAccount, DateTime fromDate, DateTime toDate)
        {
            return (bankOrCashAccountCurrencyAmounts.GetValueOrDefault(bankOrCashAccount)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetBankOrCashAccountCurrencyAmount(bankOrCashAccount, fromDate, toDate) ?? 0m);
        }

        public decimal GetEmployeeBaseAmount(Guid employee, DateTime fromDate, DateTime toDate)
        {
            return (employeeBaseAmounts.GetValueOrDefault(employee)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetEmployeeBaseAmount(employee, fromDate, toDate) ?? 0m);
        }

        public decimal GetEmployeeCurrencyAmount(Guid employee, DateTime fromDate, DateTime toDate)
        {
            return (employeeCurrencyAmounts.GetValueOrDefault(employee)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetEmployeeCurrencyAmount(employee, fromDate, toDate) ?? 0m);
        }

        public decimal GetFixedAssetAmount(Guid fixedAsset, DateTime fromDate, DateTime toDate)
        {
            return (fixedAssetBaseAmounts.GetValueOrDefault(fixedAsset)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetFixedAssetAmount(fixedAsset, fromDate, toDate) ?? 0m);
        }

        public decimal GetDepreciationAmount(Guid fixedAsset, DateTime fromDate, DateTime toDate)
        {
            return (depreciationBaseAmounts.GetValueOrDefault(fixedAsset)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetDepreciationAmount(fixedAsset, fromDate, toDate) ?? 0m);
        }

        public decimal GetIntangibleAssetAmount(Guid intangibleAsset, DateTime fromDate, DateTime toDate)
        {
            return (intangibleAssetBaseAmounts.GetValueOrDefault(intangibleAsset)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetIntangibleAssetAmount(intangibleAsset, fromDate, toDate) ?? 0m);
        }

        public decimal GetAmortizationAmount(Guid intangibleAsset, DateTime fromDate, DateTime toDate)
        {
            return (amortizationBaseAmounts.GetValueOrDefault(intangibleAsset)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetAmortizationAmount(intangibleAsset, fromDate, toDate) ?? 0m);
        }

        public decimal GetInventoryItemAmount(Guid inventoryItem, DateTime fromDate, DateTime toDate)
        {
            return (inventoryItemBaseAmounts.GetValueOrDefault(inventoryItem)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetInventoryItemAmount(inventoryItem, fromDate, toDate) ?? 0m);
        }

        public decimal GetInventoryItemQtyOwned(Guid inventoryItem, DateTime fromDate, DateTime toDate)
        {
            return (inventoryItemQtyOwned.GetValueOrDefault(inventoryItem)?.RangeSum(fromDate, toDate) ?? 0m)
                .SafeAdd(innerAggregations?.GetInventoryItemQtyOwned(inventoryItem, fromDate, toDate) ?? 0m);
        }

        public Guid[] GetBalanceSheetAccountKeys()
        {
            return [.. balanceSheetAccounts.Keys
                .Concat(innerAggregations?.GetBalanceSheetAccountKeys() ?? [])
                .Distinct()];
        }

        public Guid[] GetProfitAndLossAccountKeys()
        {
            return [.. profitAndLossAccounts.Keys
                .Concat(innerAggregations?.GetProfitAndLossAccountKeys() ?? [])
                .Distinct()];
        }

        private void UpdateBaseAmounts(Dictionary<Guid, FenwickTree> fenwickTree, Guid key, IEnumerable<GeneralLedgerTransaction> values, bool reverseSign)
        {
            Update(fenwickTree, key, [.. values.GroupBy(x => x.Date).Select(x => new Tuple<DateTime, decimal>(x.Key, reverseSign ? -x.Select(y => y.BaseAmount).SafeSum() : x.Select(y => y.BaseAmount).SafeSum()))]);
        }

        private void UpdateCurrencyAmounts(Dictionary<Guid, FenwickTree> fenwickTree, Guid key, IEnumerable<GeneralLedgerTransaction> values, bool reverseSign)
        {
            Update(fenwickTree, key, [.. values.GroupBy(x => x.Date).Select(x => new Tuple<DateTime, decimal>(x.Key, reverseSign ? -x.Select(y => y.AccountAmount).SafeSum() : x.Select(y => y.AccountAmount).SafeSum()))]);
        }

        private void UpdateQuantities(Dictionary<Guid, FenwickTree> fenwickTree, Guid key, IEnumerable<GeneralLedgerTransaction> values, bool reverseSign)
        {
            Update(fenwickTree, key, [.. values.GroupBy(x => x.Date).Select(x => new Tuple<DateTime, decimal>(x.Key, reverseSign ? -x.Select(y => y.Qty ?? 0m).SafeSum() : x.Select(y => y.Qty ?? 0m).SafeSum()))]);
        }

        private void Update(Dictionary<Guid, FenwickTree> fenwickTree, Guid key, Tuple<DateTime, decimal>[] values)
        {
            var o = fenwickTree.GetValueOrDefault(key);
            if (o != null)
            {
                o.UpdateMany(values);
            }
            else
            {
                o = new FenwickTree();
                o.UpdateMany(values);
                fenwickTree[key] = o;
            }
        }
    }
}

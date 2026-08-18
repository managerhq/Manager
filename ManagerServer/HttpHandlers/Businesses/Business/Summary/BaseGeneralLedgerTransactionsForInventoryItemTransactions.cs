using System.Linq;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInventoryItemTransactions : BaseGeneralLedgerTransactionsForNegativeInventoryClearing
    {
        protected override void InnerGet4(Context context)
        {
            if (InventoryItemQty.HasValue)
            {
                var rows = GetGeneralLedgerTransactions()
                    .Where(x => x.SubAccount?.Key == InventoryItemQty.Value)
                    .Where(x => x.Transaction != null)
                    .Where(x => x.Qty.HasValue && x.Qty.Value != 0m)
                    .OrderByDescending(x => x.Date)
                    .Select(x => new InventoryItemTransaction() { Transaction = x })
                    .ToArray();

                context.Set<Array>(rows);
            }
            else if (InventoryItemCost.HasValue)
            {
                var rows = GetGeneralLedgerTransactions()
                    .Where(x => x.SubAccount?.Key == InventoryItemCost.Value)
                    .OrderByDescending(x => x.Date)
                    .ToArray();

                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }

        public sealed class InventoryItemTransaction
        {
            public GeneralLedgerTransaction Transaction;
        }

        [Icon("fa-edit")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetEdit(InventoryItemTransaction[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction.Transaction, referrer)).ToArray();
        }

        [Icon("fa-eye")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetView(InventoryItemTransaction[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction.Transaction, referrer)).ToArray();
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("9230d564-16ee-4da3-81c2-c2b36ceaa1c2")]
        public DateTime[] GetDate(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("12cb589f-37ce-406b-bcfa-94c061745669")]
        public string[] GetTransaction(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Transaction?.GetTransactionName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("a8e451e4-ce45-4655-ba40-d54ebb2d1e13")]
        public string[] GetBankOrCashAccount(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.BankAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("3a1ee437-7f6a-481a-88ec-85fc28a44890")]
        public string[] GetExpenseClaimPayer(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.ExpenseClaimPayer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("d51aaaa8-8716-47f7-9951-1cce1290f23b")]
        public string[] GetCustomer(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Customer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("6dac138d-9fa9-4f0c-9eb0-99eb1776d091")]
        public string[] GetSupplier(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Supplier?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("8dc4c29b-dea6-4073-9fae-fdfab2acdd58")]
        public string[] GetEmployee(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Employee?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("ed4c449f-90ee-4195-9164-c2837b465402")]
        public NamedObject[] GetInventoryItem(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.InventoryItem).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("bd0bcf1b-59d7-4493-bc4b-7cf94d107fc1")]
        public string[] GetCapitalAccount(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.CapitalAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("e1dd8942-2ce1-47ee-850b-710c9761f4b5")]
        public string[] GetDescription(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Description).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, Bold, Sum]
        [Guid("32774788-7aef-436a-8e6c-027da8df2536")]
        public decimal[] GetQty(InventoryItemTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Qty.Value).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, RunningTotal2]
        public decimal[] GetBalance(InventoryItemTransaction[] rows)
        {
            var balance = rows.Sum(x => x.Transaction.Qty.Value);

            var output = new decimal[rows.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = balance;
                balance -= rows[i].Transaction.Qty.Value;
            }
            return output;
        }
    }
}
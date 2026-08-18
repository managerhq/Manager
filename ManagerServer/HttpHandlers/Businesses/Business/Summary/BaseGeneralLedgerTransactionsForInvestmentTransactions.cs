using System.Linq;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInvestmentTransactions : BaseGeneralLedgerTransactionsForInvestments
    {
        [InheritedProtoMember(350)] public Guid? Investment;

        protected override void InnerGet4(Context context)
        {
            if (Investment.HasValue)
            {
                var rows = GetGeneralLedgerTransactions()
                    .Where(x => x.SubAccount?.Key == Investment.Value)
                    .Where(x => x.Transaction != null)
                    .Where(x => x.Qty.HasValue && x.Qty.Value != 0m)
                    .OrderByDescending(x => x.Date)
                    .Select(x => new InvestmentTransaction() { Transaction = x })
                    .ToArray();

                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }

        public sealed class InvestmentTransaction
        {
            public GeneralLedgerTransaction Transaction;
        }

        [Icon("fa-edit")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetEdit(InvestmentTransaction[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction.Transaction, referrer)).ToArray();
        }

        [Icon("fa-eye")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetView(InvestmentTransaction[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction.Transaction, referrer)).ToArray();
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("5c131836-cee1-4719-9301-4ee7975e0669")]
        public DateTime[] GetDate(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("62079055-677e-4d21-94dd-b7c7b48178be")]
        public string[] GetTransaction(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Transaction?.GetTransactionName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("6d5e3cbc-ad12-4a50-b37a-98ce93c85f46")]
        public string[] GetBankOrCashAccount(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.BankAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("a338f05c-afee-44f1-a19b-c0350ef11892")]
        public string[] GetExpenseClaimPayer(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.ExpenseClaimPayer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("4bad8f27-554b-4182-85d5-fa600806ade7")]
        public string[] GetCustomer(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Customer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("34f22de3-a6a0-4748-a170-4c2ee0a5f793")]
        public string[] GetSupplier(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Supplier?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("c29afe9b-5a9c-4673-986a-de79e8c07686")]
        public string[] GetEmployee(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Employee?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("a1b21e49-fdfb-46cd-bca3-cc78c59a0b24")]
        public string[] GetInvestment(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Investment?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("c8431cfc-11f8-43e4-84ff-c5c4184a06b9")]
        public string[] GetCapitalAccount(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.CapitalAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("0348bed7-8278-4341-8419-7020fe926968")]
        public string[] GetDescription(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Description).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, Bold, Sum]
        [Guid("348c2fa6-701b-4f01-bd42-6c0ac4c9b771")]
        public decimal[] GetQty(InvestmentTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.Qty.Value).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, RunningTotal2]
        public decimal[] GetBalance(InvestmentTransaction[] rows)
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
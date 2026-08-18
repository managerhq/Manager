using ManagerServer.Model;
using ManagerServer;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsBase : BaseGeneralLedgerTransactionsAbstract
    {
        [InheritedProtoMember(400)] public Guid? GeneralLedgerAccount;
        [InheritedProtoMember(401), JsonProperty("fromDate")] public DateTime? From;
        [InheritedProtoMember(402), JsonProperty("toDate")] public DateTime To;
        [InheritedProtoMember(403)] public bool CashBasis;
        [InheritedProtoMember(405)] public Guid? Division;
        [InheritedProtoMember(406)] public bool SkipRevaluation;

        protected override void OnAfterHeader(Context context)
        {
            var unitCostColumn = context.Get<Column[]>().SingleOrDefault(x => x.Key == new Guid("526c4f4b-b924-4f1d-a333-932aa00ab3b6"));
            if (unitCostColumn != null)
            {
                unitCostColumn.Action = new Tuple<string, HttpHandler, bool>(Strings.Recalculate, new Settings.InventoryUnitCosts.InventoryCostCorrection() { Business = Business, ToDate = To, Referrer = this.ToUrl() }, false);
            }

            base.OnAfterHeader(context);
        }

        protected IEnumerable<GeneralLedgerTransaction> GetAllGeneralLedgerTransactions()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var generalLedger = new GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets();

            if (!SkipRevaluation)
            {
                generalLedger = generalLedger.Revaluate(From ?? DateTime.MinValue, To);
            }

            if (CashBasis)
            {
                var dates = new List<DateTime>();
                if (From.HasValue && From.Value > DateTime.MinValue) dates.Add(From.Value.AddDays(-1));
                dates.Add(To);
                generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().AutomaticallyMatchPurchaseInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());
            }

            var transactions = generalLedger.Where(x => x.Date <= To);

            if (Division.HasValue) transactions = transactions.Where(x => x.Division?.Key == Division.Value);

            return transactions;
        }

        protected IEnumerable<GeneralLedgerTransaction> GetGeneralLedgerTransactions()
        {
            var transactions = GetAllGeneralLedgerTransactions();

            if (GeneralLedgerAccount.HasValue) transactions = transactions = transactions.Where(x => x.BalanceSheetAccount.Key == GeneralLedgerAccount.Value || x.ProfitAndLossAccount?.Key == GeneralLedgerAccount.Value);
            if (From.HasValue) transactions = transactions.Where(x => x.Date >= From);

            return transactions;
        }

        protected override void InnerGet4(Context context)
        {
            if (context.Get<Array>() == null) context.Set<Array>(GetGeneralLedgerTransactions().OrderByDescending(x => x.Date).ToArray());
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
        [Guid("594487b2-e46c-4a1d-8d14-5603d9015bf8")]
        public DateTime[] GetDate(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("3e220c05-5731-4103-aa3e-49903df09d97")]
        public string[] GetTransaction(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetTransactionName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("8ced7c1d-12e4-4332-a21d-7358e963cda7")]
        public string[] GetAccount(GeneralLedgerTransaction[] rows)
        {
            var retainedEarningsAccount = ApplicationData.Businesses.Get(Business).Single<BalanceSheetRetainedEarningsAccount>();
            if (GeneralLedgerAccount != retainedEarningsAccount.Key) return null;
            return rows.Select(x => x.ProfitAndLossAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("a5251b08-c6c2-4139-8546-252b8dbec194")]
        public string[] GetBankOrCashAccount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BankAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("191ff2fb-95b3-4481-8563-f5ff9ed2c57c")]
        public string[] GetExpenseClaimPayer(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.ExpenseClaimPayer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("7a07810d-68b6-44c8-9c96-68ec8c38128f")]
        public string[] GetCustomer(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("1c6af28e-0df4-4b04-9e2c-911610bcbc27")]
        public string[] GetSupplier(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("8fc60480-2341-471c-9ef3-45c597fad063")]
        public string[] GetEmployee(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Employee?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("18e57e7a-8e30-4a6d-ad62-dbb079fb4f55")]
        public string[] GetInventoryKit(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryKit?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("52abfeab-4e8c-4dcf-b4ee-e26367de142d")]
        public string[] GetInventoryItem(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("7f925f01-de2c-4bbb-a6d7-ce73f6c189dc")]
        public string[] GetInvestment(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Investment?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("9dccabd1-c2fa-49a8-84e1-0da56af735b8")]
        public string[] GetFixedAsset(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.FixedAsset?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("3b7ea400-6f70-4a44-8ba7-bc9659c0c63f")]
        public string[] GetIntangibleAsset(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.IntangibleAsset?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("05b9dcb9-baf2-4b38-88cd-01ba5decd2f4")]
        public string[] GetCapitalAccount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.CapitalAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("447c80e0-547c-4eaf-9a24-6ce098e7c181")]
        public string[] GetSpecialAccount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.SpecialAccount?.GetCodeAndName()).ToArray();
        }

        [Default, HideColumnIfAllEmpty]
        [Guid("53680a9c-64d2-4ca9-b697-979b7db02e45")]
        public string[] GetDescription(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default, HideColumnIfAllEmpty, MinWidth, WhitespaceNoWrap, Center]
        [Guid("b0740469-332c-43eb-82a4-e461b99794da")]
        public string[] GetTax(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Default, HideColumnIfAllEmpty, Center, WhitespaceNoWrap]
        [Guid("94114767-7485-427e-b924-6c4440782a76")]
        public decimal?[] GetQty(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty).ToArray();
        }

        [Default, HideColumnIfAllEmpty, Right, WhitespaceNoWrap]
        [Name(nameof(Strings.AcquisitionCost))]
        [Guid("de27fcdd-5aa9-4401-b845-4baa7b89a447")]
        public decimal?[] GetPurchaseCost(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.PurchaseCost).ToArray();
        }

        [Default, HideColumnIfAllEmpty, Right, WhitespaceNoWrap]
        [Guid("526c4f4b-b924-4f1d-a333-932aa00ab3b6")]
        public Tuple<decimal, BusinessTemplate>[] GetUnitCost(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => GetHttpHandlerForUnitCost(baseCurrency, x, referrer)).ToArray();
        }

        private Tuple<decimal, BusinessTemplate> GetHttpHandlerForUnitCost(BaseCurrency baseCurrency, GeneralLedgerTransaction generalLedgerTransaction, string referrer)
        {
            if (!generalLedgerTransaction.Qty.HasValue) return null;
            if (generalLedgerTransaction.Qty.Value == 0m) return null;
            if (generalLedgerTransaction.InventoryUnitCost == null) return new Tuple<decimal, BusinessTemplate>(baseCurrency.Round(Math.Abs(generalLedgerTransaction.BaseAmount / generalLedgerTransaction.Qty.Value)), null);

            if (generalLedgerTransaction.InventoryUnitCost.Date == generalLedgerTransaction.Date) return new Tuple<decimal, BusinessTemplate>(baseCurrency.Round(generalLedgerTransaction.InventoryUnitCost.UnitCost), new InventoryUnitCostForm() { Key = generalLedgerTransaction.InventoryUnitCost.Key, Business = Business, Referrer = referrer });
            return new Tuple<decimal, BusinessTemplate>(baseCurrency.Round(generalLedgerTransaction.InventoryUnitCost.UnitCost), new InventoryUnitCostForm() { Date = generalLedgerTransaction.Date, InventoryItem = generalLedgerTransaction.InventoryItem.Key, UnitCost = generalLedgerTransaction.InventoryUnitCost.UnitCost, Business = Business, Referrer = referrer });
        }

        [Default, Right, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Name(nameof(Strings.Amount))]
        [Guid("b42cd9f4-0d0d-41d2-bb22-6bb4740ae763")]
        public Tuple<decimal, Currency>[] GetTransactionAmount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionCurrency != x.AccountCurrency ? new Tuple<decimal, Currency>(Math.Abs(x.TransactionAmount), x.TransactionCurrency) : null).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Name(nameof(Strings.Amount))]
        [Guid("d8c17c80-18ad-43d4-b8be-685095f14185")]
        public Tuple<decimal, Currency>[] GetCurrencyAmount(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.AccountCurrency is not BaseCurrency ? new Tuple<decimal, Currency>(Math.Abs(x.AccountAmount), x.AccountCurrency) : null).ToArray();
        }

        [Default, Right, Bold, Sum, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("4e1c8f59-15fb-4d5c-9b89-c9601f348618")]
        public Tuple<decimal, Currency>[] GetDebit(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.AccountAmount > 0m ? new Tuple<decimal, Currency>(x.AccountAmount, x.AccountCurrency) : null).ToArray();
        }

        [Default, Right, Bold, Sum, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("ed407725-37be-4095-95e7-220de84ee99e")]
        public Tuple<decimal, Currency>[] GetCredit(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.AccountAmount < 0m ? new Tuple<decimal, Currency>(x.AccountAmount * -1m, x.AccountCurrency) : null).ToArray();
        }        

        [Default, Right, WhitespaceNoWrap, RunningTotal2]
        public Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>[] GetBalance(GeneralLedgerTransaction[] rows)
        {
            var balance = rows.Select(x => x.AccountAmount).SafeSum();

            var output = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>[rows.Length];
            for (int i = 0; i < output.Length; i++)
            {
                if (balance >= 0m)
                {
                    output[i] = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>(balance, ManagerServer.Model.Enums.DebitCredit.Debit);
                }
                else
                {
                    output[i] = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>(balance * -1m, ManagerServer.Model.Enums.DebitCredit.Credit);
                }

                balance = balance.SafeMinus(rows[i].AccountAmount);
            }
            return output;
        }
    }
}
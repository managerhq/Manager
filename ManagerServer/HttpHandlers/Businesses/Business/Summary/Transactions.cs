using ManagerServer.Model;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    [ProtoContract]
    [Guid("e28cafe1-40fc-4859-9f70-c519c2db2ede")]
    [Title(nameof(Strings.Transactions))]
    [Guide("The `Transactions` screen displays all general ledger transactions across all accounts and all periods. This comprehensive view is useful for finding, filtering, and summarizing your transactions.")]
    [Header("Accessing Transactions")]
    [Guide("To access the `Transactions` screen, navigate to the `Summary` tab.")]
    [TabScreenshot(icon: "fa-presentation", name: nameof(Strings.Summary))]
    [Guide("Then click the `Transactions` button in the bottom-right corner of the screen.")]
    [SmallBottomButtonScreenshot(nameof(Strings.Transactions))]
    [Header("Customizing Your View")]
    [Guide("Use the `Edit Columns` button to specify which columns should be displayed in your transaction list.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithEditColumns<>))]
    [Guide("Use `Advanced Queries` to filter, sort, or group your transactions by predefined parameters.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithAdvancedQueries))]
    [Header("Exporting Data")]
    [Guide("You can use the `Copy to clipboard` button to copy transactions to an external spreadsheet program such as Excel for further analysis.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithCopyToClipboard))]
    internal sealed class Transactions : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        protected override void InnerGet4(Context context)
        {
            var rows = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).OrderByDescending(x => x.Date).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction, referrer)).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction, referrer)).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("cd39df05-3aa6-4f49-83b0-125d65d97afc")]
        public DateTime?[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date == DateTime.MinValue ? default(DateTime?) : x.Date).ToArray();
        }

        [Default]
        [Guid("bcd0f90f-1405-4ce6-82ea-55398af49473")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x.Transaction?.GetType().Name)).ToArray();
        }

        [PaddedSorting]
        [Guid("c6b65524-62e0-4b65-8b35-8b04662f3d0a")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetReference()).ToArray();
        }

        [Guid("7fcdf223-2531-43cb-9fe6-0ff354ab0bdc")]
        public string[] GetBankOrCashAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BankAccount?.GetCodeAndName()).ToArray();
        }

        [Guid("7d8136e5-b74d-4a5b-abba-f026df2c03db")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.GetCodeAndName()).ToArray();
        }

        [Guid("135220cf-7073-4e40-9af6-191d2c5e91a8")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.GetCodeAndName()).ToArray();
        }

        [Guid("b2c8b0e2-9dc2-4152-8c2f-b562d571235c")]
        public string[] GetEmployee(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Employee?.GetCodeAndName()).ToArray();
        }

        [Guid("440db414-540c-46fe-81ac-a2453265ebbd")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("ddf1cad2-e497-4c3a-9adf-e99dfb9e9456")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("c5ece916-613f-4806-8660-2086f5c885d7")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.GeneralLedgerAccount.GetCodeAndName()).ToArray();
        }

        [Guid("94630459-7140-446b-b3b5-90c1ff139504")]
        public string[] GetBalanceSheetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BalanceSheetAccount?.GetCodeAndName()).ToArray();
        }

        [Guid("6ea3df8c-a194-4c26-af70-dcfb1128de75")]
        public string[] GetProfitAndLossStatementAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.ProfitAndLossAccount?.GetCodeAndName()).ToArray();
        }

        [Guid("bdaa00e4-8794-456b-bb48-67b21990da41")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetLineDescription(x.Transaction)).ToArray();
        }

        [Guid("0aaf8545-d6e0-4f6a-9614-d7384587122f")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Right]
        [Guid("af7f6d44-9bad-4221-9116-a6f0d5f9c2cc")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetUnitPrice(x.Transaction) != null ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("7987fb90-8765-4b4c-aed8-eee6d091cbec")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("f1de9026-9721-43c7-82c9-c3e93434a13d")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("2849b685-4824-4e19-84dd-c01ab2223cde")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Right, Sum]
        [Guid("10ae929c-b6c3-4816-a8f3-0b85b2a6bb29")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {            
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, baseCurrency) : null).ToArray();
        }

        [Bold]
        [Right, Sum]
        [Guid("8df990d5-324d-48f5-a9f1-a641b5298fdf")]
        public Tuple<decimal, Currency>[] GetDebit(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.Debit.HasValue ? new Tuple<decimal, Currency>(x.Debit.Value, baseCurrency) : null).ToArray();
        }

        [Bold]
        [Right, Sum]
        [Guid("9369f83f-2728-452e-9c22-70fa48fab0d8")]
        public Tuple<decimal, Currency>[] GetCredit(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.Credit.HasValue ? new Tuple<decimal, Currency>(x.Credit.Value, baseCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("bac64ff2-eb3a-4f3a-9d90-af28e48de677")]
        public Tuple<DebitCreditAmount, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => new Tuple<DebitCreditAmount, Currency>(new DebitCreditAmount(x.BaseAmount), baseCurrency)).ToArray();
        }

        [Center]
        [MinWidth]
        [WhitespaceNoWrap]
        [Guid("54de6ff5-480c-4ddd-b9da-87746903d8a2")]
        public DateTime?[] GetTimestamp(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Transaction != null ? new DateTime(x.Transaction.Timestamp, DateTimeKind.Utc) : default(DateTime?)).ToArray();
        }
    }
}

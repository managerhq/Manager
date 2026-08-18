using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Receipts))]
    [Guid("9b0732e0-aa5a-4934-aa86-9d9b807496ab")]
    [Guide("The `Receipts` tab is where you record all money received by your business.")]
    [Guide("This includes customer payments, refunds from suppliers, interest earned, and any other incoming funds.")]
    [Guide("Each receipt transaction increases the balance in your bank or cash accounts.")]
    [Header("Recording Receipts")]
    [TabScreenshot("fa-plus-square", nameof(Strings.Receipts))]
    [Guide("To record a new receipt, click the `New Receipt` button.")]
    [HeroButtonScreenshot(nameof(Strings.Receipts), nameof(Strings.NewReceipt))]
    [Guide("While you can manually enter receipts, importing bank statements is often more efficient.")]
    [Guide("Bank imports automatically create receipt transactions, saving time and reducing errors.")]
    [Guide("You can then categorize and allocate imported transactions to the appropriate accounts.")]
    [LinkGuide("Learn more about bank imports:", typeof(BankAndCashAccounts.ImportBankStatement))]
    [Header("Managing Receipt Records")]
    [Guide("The `Receipts` tab displays your incoming transactions with detailed information in customizable columns.")]
    [Guide("Key details include dates, amounts, payers, and account allocations.")]
    [Columns]
    [Guide("Click the `Edit Columns` button to customize which columns are displayed.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn about column customization:", typeof(NakedObjectsWithEditColumns<Receipt>))]
    internal sealed class Receipts : NakedObjectsWithAutomaticRows<Receipt>
    {
        [ProtoMember(1)] public Guid? Customer;
        [ProtoMember(2)] public Guid? Supplier;

        protected override void OnAfterHeader(Context context)
        {
            var rows = context.Get<Array>() as Receipt[];

            if (rows != null && rows.Any(x => x.IsUncategorized()))
            {
                using (Div(@class: "card-header text-bg-info"))
                {
                    using (Div(@class: "flex gap-2 items-center"))
                    {
                        I(@class: "fas fa-fw fa-circle-exclamation text-neutral-400", style: "font-size: 16px");
                        using (A(href: new UncategorizedReceipts() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "font-semibold"))
                        {
                            Write(Strings.UncategorizedReceiptsAlert);
                        }
                    }
                }
            }

            base.OnAfterHeader(context);
        }

        protected override Receipt[] OnGetRows(Receipt[] rows)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                var accounts = userPermissions.GetBankCashAccounts().ToList();
                var filter = true;
                if (accounts.Count == 0)
                {
                    filter = false;
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>()) accounts.Add(e.Key);
                }
                if (filter) rows = rows.Where(x => (x.ReceivedIn.HasValue && accounts.Contains(x.ReceivedIn.Value))).ToArray();
            }

            if (Customer.HasValue) rows = rows.Where(x => x.PaidBy == ManagerServer.Model.Enums.PayerPayeeType.Customer && x.Customer == Customer).ToArray();
            if (Supplier.HasValue) rows = rows.Where(x => x.PaidBy == ManagerServer.Model.Enums.PayerPayeeType.Supplier && x.Supplier == Supplier).ToArray();

            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("1750fe19-abfd-4dc1-b8a3-6c1d1e9835c0")]
        [Guide("The date when money was received or deposited into your account.")]
        [Guide("This date affects your financial reports and cash flow tracking.")]
        [Guide("Use the actual receipt date, not when the customer sent payment.")]
        public DateTime[] GetDate(Receipt[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [WarnIfFutureDate]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("96affa31-41ea-4f63-ac22-4aa24a85ad6c")]
        [Guide("The date when this receipt appeared on your bank statement.")]
        [Guide("Cleared receipts have been confirmed by your bank and are reconciled with bank records.")]
        [Guide("Receipts without a cleared date are pending and help track deposits in transit.")]
        public DateTime?[] GetCleared(Receipt[] rows)
        {
            return rows.Select(x => x.GetClearDate()).ToArray();
        }

        [Default]
        [MinWidth]
        [PaddedSorting]
        [Guid("4119c70c-c46f-4935-b7ce-7bbdb05094fd")]
        [Guide("A unique reference number for this receipt transaction.")]
        [Guide("This could be a deposit slip number, payment reference, or transaction ID.")]
        [Guide("References help match receipts to bank statements and customer remittances.")]
        public string[] GetReference(Receipt[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("e0571522-63af-47e1-8af1-4c5409f8c3e3")]
        [Guide("The bank account, cash account, or payment method where funds were deposited.")]
        [Guide("Selecting the correct account ensures your cash balances remain accurate.")]
        [Guide("This determines which account balance increases from the receipt.")]
        public NamedObject[] GetReceivedIn(Receipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.ReceivedIn)).ToArray();
        }

        [Default]
        [Guid("5ccff5b0-fc25-4541-a500-8436dc2f5c3f")]
        [Guide("A brief description explaining what this receipt was for.")]
        [Guide("Include details like invoice numbers paid, service period, or payment purpose.")]
        [Guide("Clear descriptions help identify transactions when reviewing records later.")]
        public string[] GetDescription(Receipt[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default]
        [Guid("403c7f16-0b50-45d2-bf56-f2946269bca1")]
        [Guide("The person or business who paid you this money.")]
        [Guide("This could be a customer paying an invoice, a supplier issuing a refund, or any other payer.")]
        [Guide("Accurate payer information helps track customer payments and generate receivables reports.")]
        public string[] GetPaidBy(Receipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.Contact).ToArray();
        }

        [Guid("d4fa1012-4fd6-4f83-80ce-58f63fe92dcf")]
        [Guide("The income or asset accounts that categorize the source of this receipt.")]
        [Guide("Proper categorization ensures accurate financial statements and income tracking.")]
        [Guide("Multiple accounts indicate the receipt was split between different income sources.")]
        public string[] GetAccounts(Receipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => !x.IsBalancing).Select(x => x.Account).Distinct())).ToArray();
        }

        [Guid("5adcd270-2a2b-44d2-8459-7a48caacc8ff")]
        [Guide("Shows which projects or jobs generated this income, if using project tracking.")]
        [Guide("Project allocation helps track revenue and profitability by project.")]
        [Guide("This column only appears when the `Projects` tab is activated in your business.")]
        [LinkGuide("For more information, see:", typeof(Projects.Projects))]
        public string[] GetProject(Receipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Project != null).Select(x => x.Project.Name).Distinct())).ToArray();
        }

        [Default]
        [Right, Sum]
        [HideColumnIfAllEmpty]
        [Guid("da498e45-d326-48c4-9744-ba1958c6faed")]
        [Guide("Shows the cost of inventory items sold in this transaction.")]
        [Guide("This automatic calculation helps track gross profit on inventory sales.")]
        [Guide("Cost of sales reduces your inventory value and increases your expense accounts.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetCostOfSales(Receipt[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.CostOfSales(database).HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(x.CostOfSales(database).Value, baseCurrency, new ReceiptCosts() { Business = Business, Transaction = x.Key, ReverseSign = true, Referrer = referrer }) : null).ToArray();
        }

        [Sum]
        [Bold]
        [Right]
        [Default]
        [Guid("d28141b9-2052-44c6-898d-3a3c3cae7ded")]
        [Guide("The total amount of money received in this transaction.")]
        [Guide("For foreign currency receipts, both the foreign amount and base currency equivalent are displayed.")]
        [Guide("This amount increases your bank account balance and affects income accounts or reduces liabilities.")]
        public Tuple<decimal, Currency>[] GetAmount(Receipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!Customer.HasValue && !Supplier.HasValue)
            {
                using (A(href: new ReceiptsFindAndRecode() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.FindAndRecode);
                using (A(href: new ReceiptLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.Receipt + " - " + Strings.Lines);
            }
            base.OnFooterEndSection(context);
        }
    }
}

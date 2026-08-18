using ManagerServer.Model;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Guid("7d4ddecc-559b-4783-8f4a-e7f3865faba9")]
    [Title(nameof(Strings.Receipts), nameof(Strings.Lines))]
    [Guide("The `Receipts - Lines` screen displays a detailed list of all receipt line items from all receipts. This view is useful for analyzing, filtering, and finding specific receipt entries based on their line items.")]
    [Guide("To reach the `Receipts - Lines` screen, navigate to the `Receipts` tab.")]
    [TabScreenshot("fa-plus-square", nameof(Strings.Receipts))]
    [Guide("Then click the `Receipts - Lines` button at the bottom of the screen.")]
    [SmallBottomButtonScreenshot("Receipts-Lines")]
    [Guide("The `Receipts - Lines` screen displays information in columns that can be customized to show the data most relevant to your needs.")]
    [Columns]
    [Guide("Click the `Edit Columns` button to select which columns to display or hide. This allows you to customize the view to show only the information you need.")]
    internal sealed class ReceiptLines : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.Receipt.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var receipts = database.OfType<Receipt>();
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
                if (filter) receipts = receipts.Where(x => (x.ReceivedIn.HasValue && accounts.Contains(x.ReceivedIn.Value))).ToArray();
            }

            var rows = receipts.SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new ReceiptForm() { Business = Business, Key = x.Receipt.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new ReceiptView() { Business = Business, Key = x.Receipt.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WhitespaceNoWrap]
        [WarnIfFutureDate, MinWidth, Center]
        [Guid("d78596a1-4159-4996-bacd-d62390b68da7")]
        [Guide("The date when the money was received. This field records when the funds were actually deposited into your bank or cash account.")]
        [Guide("Use this date for reconciling bank statements and tracking cash inflows. The receipt date determines which accounting period the transaction belongs to.")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Receipt.Date).ToArray();
        }

        [PaddedSorting]
        [Guid("4d880013-ae45-469b-b57b-4dfa3e75a311")]
        [Guide("The unique identifier or reference number for the receipt. This helps you track and identify specific receipts in your records.")]
        [Guide("Use sequential numbering or other meaningful reference systems. This field is essential for matching receipts with bank deposits and maintaining proper documentation.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Receipt.Reference).ToArray();
        }

        [Default]
        [Guid("e5caaf17-e902-4f9a-a16c-5d3f1aced061")]
        [Guide("The bank or cash account where the money was received. This indicates where the funds were deposited.")]
        [Guide("Select the appropriate account to ensure accurate cash flow tracking and bank reconciliation. The account balance will be increased by the receipt amount.")]
        public string[] GetBankOrCashAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BankAccount?.Name).ToArray();
        }

        [Guid("eb81f6b8-18ab-4dd9-98e1-ea839ce84e7f")]
        [Guide("The customer who made the payment. This identifies the source of the funds and updates their account balance.")]
        [Guide("Select the correct customer to ensure accurate accounts receivable tracking and customer statements. This field is typically used when receiving payments for sales invoices.")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("a74b4b37-3360-4173-b386-afcb035761b8")]
        [Guide("The supplier associated with the receipt, if applicable. This is used when receiving refunds from suppliers or other supplier-related receipts.")]
        [Guide("Select a supplier when the receipt relates to supplier refunds, rebates, or other supplier transactions. This helps track supplier account balances.")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("546d43b3-fa66-4cd6-8818-7e6ba55b6b99")]
        [Guide("A brief description or explanation of what the receipt is for. This helps identify the source and purpose of the money received.")]
        [Guide("Examples: 'Payment from customer invoice #123', 'Interest income', 'Refund from supplier'. Keep descriptions clear and specific for easy searching and reporting.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Receipt.Description).ToArray();
        }

        [Guid("b806f42c-4e3b-4e0e-a2a6-480d83779c16")]
        [Guide("The inventory or non-inventory item associated with the receipt line. This links the receipt to specific products or services sold.")]
        [Guide("When an item is selected, its default income account and tax settings will be applied automatically. Leave blank for receipts not related to specific items.")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("24e143d5-7e9a-49c6-a135-5d50df9bdd9b")]
        [Guide("The general ledger account that the receipt line is posted to. This determines how the income or reduction in liability is categorized.")]
        [Guide("Choose the appropriate income, asset, or liability account based on what you are receiving payment for. The account selection affects financial reports and tax calculations.")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("1eefa2f2-b452-49b8-a583-53eacc4a5c81")]
        [Guide("A detailed description for the specific line item within the receipt. This provides context for what this portion of the receipt represents.")]
        [Guide("Include specifics like invoice numbers being paid, service periods, or item details. Clear line descriptions help you understand the receipt without referring to other documents.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Guid("7488a8d6-f631-4de1-b257-8b45305180df")]
        [Guide("The number of units sold or services provided in the line item. This is used when receiving payment for countable items or services.")]
        [Guide("Enter quantities when receiving payment for inventory items, hours of service, or other measurable units. The quantity multiplied by the unit price determines the line total.")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Right]
        [Guid("fb3fda76-80df-4a39-a393-3499b0a4ea64")]
        [Guide("The price per unit for items or services on the receipt line. This is the rate charged to the customer.")]
        [Guide("The unit price should match what was quoted or invoiced to the customer. When multiplied by the quantity, it calculates the line amount before any taxes.")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("925136f8-5987-4c59-9bc3-8fd882f52d73")]
        [Guide("The project that the receipt line should be allocated to for tracking purposes. This enables project income tracking and profitability analysis.")]
        [Guide("Assign receipts to projects to track project revenue and determine project profitability. This is essential for job costing and project-based businesses.")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("64ec1d3e-a9ff-46f8-8487-36db9a9f4eb8")]
        [Guide("The business division or department that the receipt line belongs to. This enables income tracking by organizational unit.")]
        [Guide("Use divisions to track revenue by department, location, or business segment. This helps analyze income and profitability by division for better management decisions.")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("3f9d6223-d138-44ff-ade9-9f22a2ca51cf")]
        [Guide("The tax code applied to the receipt line, which determines the tax calculation and treatment for this income.")]
        [Guide("Select the appropriate tax code based on the type of income and tax regulations. The tax code determines whether tax is added to the amount and at what rate.")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Right, Sum]
        [Guid("c5811424-aa73-480a-b70c-0ad5075ddcda")]
        [Guide("The tax amount collected on the receipt line. This shows the tax component of the payment received.")]
        [Guide("For tax-inclusive amounts, this shows the tax portion already included. For tax-exclusive amounts, this is added to the line subtotal. Tax amounts affect your output tax liability calculations.")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value * -1m, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("8d76964b-6863-4464-8981-eb358539db4a")]
        [Guide("The total monetary value received for the line item. This represents the actual amount collected for this specific line.")]
        [Guide("This is calculated as quantity × unit price plus any applicable taxes, or entered directly for non-quantity-based receipts. The sum of all line amounts equals the total receipt.")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount*-1m, x.TransactionCurrency)).ToArray();
        }
    }
}

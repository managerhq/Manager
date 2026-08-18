using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{
    [ProtoContract]
    [Guid("27be4f13-42f9-401f-aaad-e02cbeef3423")]
    [Title(nameof(Strings.Payments), nameof(Strings.Lines))]
    [Guide("The `Payments - Lines` screen displays individual line items from all payments in your business. This provides a detailed view of payment transactions, making it easy to search, filter, and analyze specific payment details.")]
    [Header("Navigation")]
    [Guide("To access the `Payments - Lines` screen, navigate to the `Payments` tab in the main menu.")]
    [TabScreenshot("fa-minus-square", nameof(Strings.Payments))]
    [Guide("Click the `Payments - Lines` button at the bottom of the payments list.")]
    [SmallBottomButtonScreenshot("Payments-Lines")]
    [Header("Column Management")]
    [Guide("The screen displays payment line data in columns. You can customize which columns appear to focus on the information most relevant to your needs.")]
    [Columns]
    [Guide("Click the `Edit Columns` button to select which columns to display. This lets you create custom views tailored to specific reporting or analysis requirements.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more about customizing columns:", typeof(NakedObjectsWithEditColumns<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>))]
    [Header("Advanced Queries")]
    [Guide("Use `Advanced Queries` to create powerful custom reports and analyses. This feature allows you to filter, group, and summarize payment data in sophisticated ways.")]
    [Guide("For example, to view total payments by supplier for accounts payable transactions only, you can create a query that filters by account type and groups by supplier. This helps analyze spending patterns and supplier relationships.")]
    [AdvancedQuery(select: new[] { nameof(Strings.Supplier), nameof(Strings.Amount) }, where: new[] { nameof(Strings.Account), nameof(Strings.Is), nameof(Strings.AccountsPayable) }, groupBy: new[] { nameof(Strings.Supplier) })]
    internal sealed class PaymentLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.Payment.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var payments = database.OfType<Payment>();
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
                if (filter) payments = payments.Where(x => (x.PaidFrom.HasValue && accounts.Contains(x.PaidFrom.Value))).ToArray();
            }

            var rows = payments.SelectMany(x => x.GetGeneralLedgerTransactions(database)).Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PaymentForm() { Business = Business, Key = x.Payment.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PaymentView() { Business = Business, Key = x.Payment.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("1fc977c7-fd89-4674-9614-11260308269b")]
        [Guide("The date when the payment was made. This records the actual disbursement date of funds from your bank or cash account.")]
        [Guide("Ensure the date is accurate as it affects bank reconciliation, cash flow tracking, financial reporting, and tax calculations.")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Payment.Date).ToArray();
        }

        [Default]
        [Guid("d295aba3-7baf-4863-8701-0e37ce708d63")]
        [Guide("A unique reference number or identifier for the payment. This helps you track and locate specific payments quickly.")]
        [Guide("Common references include check numbers, electronic transfer IDs, or sequential payment numbers. Consistent referencing improves bank reconciliation and maintains a clear audit trail.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Payment.Reference).ToArray();
        }

        [Default]
        [Guid("0dff0f8c-542b-4660-a01c-dc0c55f3a0bd")]
        [Guide("The bank or cash account used to make this payment. This shows where the funds came from.")]
        [Guide("Selecting the correct account ensures accurate bank reconciliation and cash flow reporting. The payment amount will be deducted from this account's balance.")]
        public string[] GetBankOrCashAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BankAccount?.Name).ToArray();
        }

        [Guid("9cb7aee2-58ab-4d7a-bb6b-415fba2a97ac")]
        [Guide("The customer associated with this payment. Used for customer refunds, credit balance returns, or other customer-related payments.")]
        [Guide("Selecting a customer updates their account balance and maintains accurate customer statements. Leave blank if the payment is not customer-related.")]
        public string[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer?.Name).ToArray();
        }

        [Guid("31ffa0bb-d3de-491a-9d98-ddefbd316f6b")]
        [Guide("The supplier or vendor receiving this payment. This identifies the payment recipient and updates their account balance.")]
        [Guide("Choose the correct supplier to maintain accurate accounts payable records and supplier statements. Most commonly used for purchase invoice payments and vendor expense reimbursements.")]
        public string[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier?.Name).ToArray();
        }

        [Guid("954f0abf-2120-49e9-b930-97841cdfeb92")]
        [Guide("A brief description explaining the purpose of this payment that appears in lists and reports for quick identification.")]
        [Guide("Use clear, specific descriptions like \"Office rent - March 2024\" or \"Invoice #12345 payment\". Good descriptions make searching and reporting much easier.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Payment.Description).ToArray();
        }

        [Guid("e830043e-2df3-472d-84cb-6b6aae8ad18a")]
        [Guide("The inventory or non-inventory item this payment line relates to, linking the payment to specific products or services in your item list.")]
        [Guide("Selecting an item automatically applies its default account and tax code settings. Leave this field blank for general expenses not tied to specific items.")]
        public string[] GetItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Item?.GetNameWithCode()).ToArray();
        }

        [Default]
        [Guid("de75f511-f547-4666-9507-d420fdf07369")]
        [Guide("The general ledger account where this payment line will be posted. This categorizes the transaction in your accounting system.")]
        [Guide("Select the appropriate expense, asset, or liability account based on the payment purpose. This choice directly impacts your financial statements and tax reporting.")]
        public string[] GetAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Account).ToArray();
        }

        [Guid("4138009f-a942-4131-911e-13e5f8e9a2e6")]
        [Guide("A detailed description for this specific line item that provides additional context about what this portion of the payment covers.")]
        [Guide("Include relevant details such as invoice numbers, service periods, or specific work performed. Detailed line descriptions eliminate the need to reference source documents later.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Guid("c7112608-ead8-4781-9b9d-5aaef03a0fcb")]
        [Guide("The quantity of units being paid for in this line item. Used for countable items or measurable services.")]
        [Guide("Enter the number of items, hours, or other units. The system calculates the line total by multiplying quantity by unit price.")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value*-1m : default(decimal?)).ToArray();
        }

        [Right]
        [Guid("681810be-1a98-46f4-8e16-458fcadeafbc")]
        [Guide("The price per unit for this line item, used when purchasing specific quantities of goods or services.")]
        [Guide("This could represent the cost per item, hourly rate for services, or price per unit of measure. The system multiplies this by quantity to calculate the line total.")]
        public Tuple<decimal, Currency>[] GetUnitPrice(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetUnitPrice(x.Transaction).HasValue ? new Tuple<decimal, Currency>(x.TransactionLine.GetUnitPrice(x.Transaction).Value, x.TransactionCurrency) : null).ToArray();
        }

        [Guid("bb5d4a01-11d9-443f-81ce-d17bf77626e8")]
        [Guide("The project this payment line is allocated to. Enables tracking of project-specific costs and profitability.")]
        [Guide("Assigning payments to projects helps monitor project budgets, analyze profitability, and prepare project-based financial reports. Essential for businesses that track job costs.")]
        public string[] GetProject(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Project?.Name).ToArray();
        }

        [Guid("31202a9d-bd95-4172-be99-2fad9f3a8e2c")]
        [Guide("The division or department this payment line belongs to, enabling expense tracking by organizational unit.")]
        [Guide("Divisions help analyze costs by department, location, or business segment. This segmentation supports better budgeting and profitability analysis for each division.")]
        public string[] GetDivision(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Division?.Name).ToArray();
        }

        [Guid("08b04319-8022-4800-9fb4-89f4ffd5b363")]
        [Guide("The tax code applied to this payment line. Determines the tax treatment and rate for this expense.")]
        [Guide("Choose the correct tax code to ensure proper tax calculations and reporting. Tax codes determine whether tax is recoverable, the tax rate, and how it appears in tax reports.")]
        public string[] GetTaxCode(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxCode?.Name).ToArray();
        }

        [Right, Sum]
        [Guid("a18d40da-00fb-4dce-a745-18051159bed8")]
        [Guide("The tax amount for this payment line. Shows the tax component calculated based on the tax code.")]
        [Guide("For tax-inclusive pricing, this displays the tax portion already included in the amount. For tax-exclusive pricing, this tax is added to the subtotal. Tax amounts flow to your tax reports and affect input tax credits.")]
        public Tuple<decimal, Currency>[] GetTaxAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.TaxAmount.HasValue ? new Tuple<decimal, Currency>(x.TaxAmount.Value, x.TransactionCurrency) : null).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("e9b8f61a-bbff-4dee-b07e-623ab2089a91")]
        [Guide("The total amount for this payment line. Represents the complete value including any applicable taxes.")]
        [Guide("Calculated as quantity × unit price for quantity-based items, or entered directly for fixed amounts. The sum of all line amounts equals the total payment value.")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => new Tuple<decimal, Currency>(x.TransactionAmount, x.TransactionCurrency)).ToArray();
        }
    }
}

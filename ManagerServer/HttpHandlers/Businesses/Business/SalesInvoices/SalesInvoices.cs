using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;
using ManagerServer.HttpHandlers.Businesses.Business.InventoryWriteOffs;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{    
    [Title(nameof(Strings.SalesInvoices))]
    [Guide("The `Sales Invoices` tab is where you create and manage invoices to bill customers for goods sold or services provided.")]
    [Guide("Each invoice increases the customer's balance in `Accounts Receivable`, representing money they owe you.")]
    [Guide("From this tab, you can track payment status, send invoices to customers, and monitor overdue accounts.")]
    [TabScreenshot("fa-file-invoice", nameof(Strings.SalesInvoices))]
    [Header("Creating Sales Invoices")]
    [Guide("To create a new sales invoice, click the `New Sales Invoice` button.")]
    [HeroButtonScreenshot(nameof(Strings.SalesInvoices), nameof(Strings.NewSalesInvoice))]
    [LinkGuide("Learn more:", typeof(SalesInvoiceForm))]
    [Header("Inventory Management")]
    [Guide("When you invoice inventory items, Manager automatically updates your inventory quantities:")]
    [Guide("• `Qty Owned` decreases because you've sold the items")]
    [Guide("• `Qty to Deliver` increases because you still need to ship them")]
    [Guide("To record the actual delivery, create a delivery note under the `Delivery Notes` tab. This will reduce both `Qty on Hand` and `Qty to Deliver`.")]
    [LinkGuide("Learn more:", typeof(DeliveryNotes.DeliveryNotes))]
    [Guide("For immediate delivery sales, you can combine invoicing and delivery in one step:")]
    [Guide("• Check the `Acts as Delivery Note` checkbox when creating the invoice")]
    [Guide("• Select the `Inventory Location` from which items are being shipped")]
    [Guide("• This will decrease `Qty on Hand` immediately instead of creating a delivery obligation")]
    [Header("Customizing Columns")]
    [Guide("The `Sales Invoices` tab features several columns.")]
    [Columns]
    [Guide("Click the `Edit Columns` button to choose which columns you want to display.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more:", typeof(NakedObjectsWithEditColumns<SalesInvoice>))]
    [Header("Advanced Queries")]
    [Guide("The `Advanced Queries` feature provides powerful tools to analyze your sales invoice data.")]
    [Guide("For example, to focus on collection efforts, you can view only overdue invoices sorted by days overdue:")]
    [AdvancedQuery(select: new[] { nameof(Strings.IssueDate), nameof(Strings.Reference), nameof(Strings.Customer), nameof(Strings.InvoiceAmount), nameof(Strings.BalanceDue), nameof(Strings.DaysOverdue), nameof(Strings.Status) }, where: new[] { nameof(Strings.Status), nameof(Strings.Is), nameof(Strings.Overdue) }, orderBy: new[] { nameof(Strings.DaysOverdue), nameof(Strings.Descending) })]
    [Guide("Another useful query groups invoices by customer to show total sales for each:")]
    [AdvancedQuery(select: new[] { nameof(Strings.Customer), nameof(Strings.InvoiceAmount) }, groupBy: new[] { nameof(Strings.Customer) })]
    [Guide("These are just two examples. You can create queries to analyze sales trends, identify top customers, track performance by division, monitor cash flow, and much more.")]
    [Guide("All columns, including custom fields, can be used in your queries for maximum flexibility.")]
    [LinkGuide("Learn more:", typeof(NakedObjectsWithAdvancedQueries))]

    [ProtoContract]
    [NamespaceEntry]
    [Guid("8182cc84-d4ca-45c6-b159-df07ac0523e5")]
    internal class SalesInvoices : NakedObjectsWithAutomaticRows<SalesInvoice>
    {
        [ProtoMember(1)] public Guid? Customer;
        [ProtoMember(2)] public Guid? SalesOrder;

        protected override SalesInvoice[] OnGetRows(SalesInvoice[] rows)
        {
            if (Customer.HasValue) rows = rows.Where(x => x.Customer == Customer.Value).ToArray();
            if (SalesOrder.HasValue) rows = rows.Where(x => x.SalesOrder == SalesOrder.Value).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center, WhitespaceNoWrap, MinWidth]
        [Guid("7d9fb8d6-ed51-484d-a061-deedde82573e")]
        [Guide("The `Issue Date` column shows when the invoice was created.")]
        [Guide("This date determines when the sale is recorded in your accounts and affects due date calculations.")]
        public DateTime[] GetIssueDate(SalesInvoice[] rows)
        {
            return rows.Select(x => x.IssueDate).ToArray();
        }

        [Guid("2c167da6-f8c3-4891-9ba1-18be7527b81e")]
        [Guide("The `Due Date` column indicates when payment is expected from the customer.")]
        [Guide("This date is calculated automatically based on your payment terms or can be set manually.")]
        [Guide("Invoices past this date will show as overdue.")]
        public DateTime[] GetDueDate(SalesInvoice[] rows)
        {
            return rows.Select(x => x.GetDueDate()).ToArray();
        }

        [Default]
        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("296af040-823b-41d2-ad34-48e3bb649333")]
        [Guide("The `Reference` column contains the unique invoice number.")]
        [Guide("This reference appears on the printed invoice and helps both you and your customer identify specific transactions.")]
        public string[] GetReference(SalesInvoice[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [PaddedSorting]
        [Guid("386efd08-216e-44c8-b88d-f6ce54286480")]
        [Guide("The `Sales Quote` column shows which quote this invoice was created from.")]
        [Guide("This helps you track the conversion of quotes to actual sales.")]
        public string[] GetSalesQuote(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SalesQuote>(x.SalesQuote)?.GetName()).ToArray();
        }

        [PaddedSorting]
        [Guid("dfd97710-adc5-4595-8452-b152eb099065")]
        [Guide("The `Sales Order` column indicates which order this invoice fulfills.")]
        [Guide("This links the invoice back to the original customer order for complete transaction tracking.")]
        public string[] GetSalesOrder(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SalesOrder>(x.SalesOrder)?.GetName()).ToArray();
        }

        [Default]
        [Guid("a8e51391-9898-4e02-8b96-6e57c5988677")]
        [Guide("The `Customer` column shows who this invoice was issued to.")]
        [Guide("The customer name links to their full record where you can see all their transactions and current balance.")]
        public string[] GetCustomer(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("3396d079-05ba-4a6f-b1b3-7cf5f8b753a4")]
        [Guide("The `Description` column displays a summary description for the entire invoice.")]
        [Guide("This is useful for providing context about what the invoice covers overall.")]
        [Guide("For detailed line-by-line descriptions, view the full invoice or use the invoice lines report.")]
        [LinkGuide("For more information, see:", typeof(SalesInvoiceLines))]
        public string[] GetDescription(SalesInvoice[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("d22ac1a6-06b1-4437-9081-d952a14e2a21")]
        [Guide("The `Project` column shows which projects are billed on this invoice.")]
        [Guide("Since projects are assigned per line item, one invoice can bill for multiple projects.")]
        [Guide("All project names are listed when an invoice spans multiple projects.")]
        public string[] GetProject(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Project != null).Select(x => x.Project.Name).Distinct())).ToArray();
        }

        [Guid("cda0f956-6090-4047-8a2c-c99ba0755bf5")]
        [Guide("The `Division` column indicates which divisions are involved in this invoice.")]
        [Guide("Since divisions are assigned per line item, one invoice can include sales from multiple divisions.")]
        [Guide("All division names are listed when an invoice spans multiple divisions.")]
        public string[] GetDivision(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Division != null).Select(x => x.Division.Name).Distinct())).ToArray();
        }

        [Guid("86e0d29b-97a5-4086-87ea-141f833d485a")]
        public bool[] GetClosedInvoice(SalesInvoice[] rows)
        {
            return rows.Select(x => x.ClosedInvoice).ToArray();
        }

        [Right, Sum]
        [Guid("7d8a20c3-5fc8-495b-83f0-1a9d7d660644")]
        [Guide("The `Withholding Tax` column shows tax amounts that the customer will withhold from their payment.")]
        [Guide("In some jurisdictions, customers are required to withhold tax and pay it directly to tax authorities.")]
        [Guide("This amount reduces what the customer actually pays you but creates a tax credit you can claim.")]
        public Tuple<decimal, Currency>[] GetWithholdingTax(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.GeneralLedgerAccount is BalanceSheetWithholdingTaxReceivableAccount)?.GetTransactionAmountWithCurrency()).ToArray();
        }

        [Right, Sum]
        [Guid("f5fd6df5-5a42-46fb-8ae9-46cead2a66fd")]
        [Guide("The `Discount` column shows the total discount amount given across all line items.")]
        [Guide("Discounts can be applied as percentages or fixed amounts on individual lines.")]
        [Guide("This total helps you track the revenue impact of discounts offered to customers.")]
        public Tuple<decimal, Currency>[] GetDiscount(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new Tuple<decimal, Currency>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                var transactions = rows[i].GetGeneralLedgerTransactions(database);
                var transactionCurrency = rows[i].GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.TransactionCurrency;
                var totalDiscount = transactions.Sum(x => x.Discount);
                if (totalDiscount != 0m)
                {
                    output[i] = new Tuple<decimal, Currency>(totalDiscount, transactionCurrency);
                }
            }
            return output;
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("fd824997-c2fa-4a44-9cee-c13c09942d5f")]
        [Guide("The `Invoice Amount` column shows the total amount billed to the customer.")]
        [Guide("This includes all line items, taxes, and fees, minus any discounts.")]
        [Guide("This is the amount the customer needs to pay (before any withholding tax).")]
        public Tuple<decimal, Currency>[] GetInvoiceAmount(SalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        private Dictionary<SalesInvoice, Balance> getBalances = null;
        public Dictionary<SalesInvoice, Balance> GetBalances(SalesInvoice[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();
                var customers = rows.Where(x => x.Customer.HasValue).Select(x => x.Customer.Value).Distinct().ToArray();
                var salesInvoices = new HashSet<Guid>(rows.Select(x => x.Key));
                var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices(customers);
                var balances = generalLedger.Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.SalesInvoice != null && salesInvoices.Contains(x.SalesInvoice.Key)).GroupBy(x => x.SalesInvoice.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount));

                var output = new Dictionary<SalesInvoice, Balance>();
                foreach (var e in rows)
                {
                    var currency = database.SingleOrDefault<ManagerServer.Model.Currency>(database.SingleOrDefault<ManagerServer.Model.Customer>(e.Customer)?.Currency) as ManagerServer.Model.Currency ?? baseCurrency;

                    var invoiceBalance = balances.TryGetValue(e.Key, out decimal value) ? value : 0m;
                    var status = BalanceStatus.PaidInFull;
                    if (invoiceBalance < 0m) status = BalanceStatus.Overpaid;
                    if (invoiceBalance > 0m) status = BalanceStatus.ComingDue;

                    int? daysOverdue = null;
                    int? daysToDueDate = null;
                    if (status == BalanceStatus.ComingDue)
                    {
                        daysOverdue = (int?)((DateTime.Today.Ticks - e.GetDueDate().Ticks) / TimeSpan.TicksPerDay);
                        if (daysOverdue == 0)
                        {
                            daysOverdue = null;
                            status = BalanceStatus.DueToday;
                        }
                        else if (daysOverdue < 0)
                        {
                            daysToDueDate = daysOverdue.Value * -1;
                            daysOverdue = null;
                        }
                    }

                    if (daysOverdue.HasValue)
                    {
                        if (daysOverdue.Value == 1) status = BalanceStatus.DueYesterday;
                        else status = BalanceStatus.Overdue;
                    }

                    if (daysToDueDate.HasValue)
                    {
                        if (daysToDueDate.Value == 1) status = BalanceStatus.DueTomorrow;
                    }

                    output.Add(e, new Balance()
                    {
                        BalanceDue = new Tuple<decimal, Currency, BusinessTemplate>(invoiceBalance, currency, new SalesInvoiceTransactions() { Business = Business, SalesInvoice = e.Key, Referrer = referrer }),
                        Status = status,
                        DaysOverdue = daysOverdue,
                        DaysToDueDate = daysToDueDate
                    });
                }
                getBalances = output;
            }
            return getBalances;
        }

        [Default]
        [Right, Sum]
        [HideColumnIfAllEmpty]
        [Guid("a0d5dbf5-ed08-4d05-94df-3136441504ce")]
        [Guide("The `Cost of Sales` column displays the total cost of inventory items sold on this invoice.")]
        [Guide("This helps you see the gross profit on each invoice by comparing it to the invoice amount.")]
        [Guide("Only appears when the invoice includes inventory items with cost tracking.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetCostOfSales(SalesInvoice[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows.Select(x => x.CostOfSales(database).HasValue ? new Tuple<decimal, Currency, BusinessTemplate>(x.CostOfSales(database).Value, baseCurrency, new SalesInvoiceCosts() { Business = Business, Transaction = x.Key, ReverseSign = true, Referrer = referrer }) : null).ToArray();
        }

        [Right, Sum, WarnIfNegative]
        [Guid("df1b940c-3d22-4303-86e9-aac80da0ee3e"), Default]
        [Guide("The `Balance Due` column shows the outstanding amount the customer still owes on this invoice.")]
        [Guide("This balance decreases as customers make payments or credits are applied.")]
        [Guide("Click the amount to see a detailed history of all payments and adjustments.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetBalanceDue(SalesInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].BalanceDue).ToArray();
        }

        [Center]
        [Guid("f83e0aa5-a155-49e6-9972-b02d045a378c")]
        [Guide("The `Days to Due Date` column shows how many days remain before payment is due from the customer.")]
        [Guide("This countdown helps you anticipate incoming cash flow and send payment reminders.")]
        [Guide("Once the due date passes, this column becomes blank and days overdue begins counting.")]
        public int?[] GetDaysToDueDate(SalesInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].DaysToDueDate).ToArray();
        }

        [Center]
        [Guid("df47af2a-c361-46e3-8704-5bc82d39c0b7")]
        [Guide("The `Days Overdue` column shows how many days have passed since the invoice due date.")]
        [Guide("Use this to prioritize collection efforts - the higher the number, the older the debt.")]
        [Guide("Consider following up with customers when invoices become overdue to ensure timely payment.")]
        public int?[] GetDaysOverdue(SalesInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].DaysOverdue).ToArray();
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("41cae1bb-270d-4678-8716-0697bf362650"), Default]
        [Guide("The `Status` column shows the payment status of each invoice with color-coded indicators.")]
        [Guide("Green means fully paid, yellow indicates payment is coming due, and red signals overdue.")]
        [Guide("This visual system helps you quickly identify which invoices need your attention.")]
        public BalanceStatus[] GetStatus(SalesInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].Status).ToArray();
        }

        public sealed class Balance
        {
            public Tuple<decimal, Currency, BusinessTemplate> BalanceDue;
            public int? DaysToDueDate;
            public int? DaysOverdue;
            public BalanceStatus Status;            
        }

        public enum BalanceStatus
        {
            [Success] PaidInFull,
            [Warning] ComingDue,
            [Warning] DueToday,
            [Warning] DueTomorrow,
            [Danger] DueYesterday,
            [Danger] Overdue,
            Overpaid
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!Customer.HasValue && !SalesOrder.HasValue)
            {
                using (A(href: new SalesInvoiceLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.SalesInvoices + " - " + Strings.Lines);
            }
#if DEBUG
            using (A(href: new SalesInvoiceReconciliation() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.SalesInvoices + " - " + Strings.Reconciliation);
#endif
            base.OnFooterEndSection(context);
        }
    }
}

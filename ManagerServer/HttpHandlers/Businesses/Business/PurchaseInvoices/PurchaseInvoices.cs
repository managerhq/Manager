using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.PurchaseInvoices))]
    [Guid("dc26148e-e7f7-4f0c-be66-83de8139ec96")]
    [Guide("The **Purchase Invoices** tab is where you record invoices received from suppliers for goods or services purchased.")]
    [Guide("Each invoice you enter increases the supplier's balance in *Accounts Payable*, representing money you owe them.")]
    [Guide("From this tab, you can track payment due dates, manage cash flow, and ensure accurate expense recording.")]
    [TabScreenshot("fa-file-invoice", nameof(Strings.PurchaseInvoices))]
    [Header("Getting Started")]
    [Guide("To create a new purchase invoice, click the **New Purchase Invoice** button.")]
    [HeroButtonScreenshot(nameof(Strings.PurchaseInvoices), nameof(Strings.NewPurchaseInvoice))]
    [LinkGuide("For more information, see:", typeof(PurchaseInvoiceForm))]
    [Header("Understanding the Display")]
    [Guide("The **Purchase Invoices** tab displays key information about each invoice in organized columns.")]
    [Guide("You can customize which columns appear and use advanced queries to analyze your payables.")]
    [Columns]
    internal class PurchaseInvoices : NakedObjectsWithAutomaticRows<PurchaseInvoice>
    {
        [ProtoMember(1)] public Guid? Supplier;
        [ProtoMember(2)] public Guid? PurchaseOrder;

        protected override PurchaseInvoice[] OnGetRows(PurchaseInvoice[] rows)
        {
            if (Supplier.HasValue) rows = rows.Where(x => x.Supplier == Supplier.Value).ToArray();
            if (PurchaseOrder.HasValue) rows = rows.Where(x => x.PurchaseOrder == PurchaseOrder.Value).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("e62cb3bd-ae1a-49f1-8dfb-eaba002966e6")]
        [Guide("The **Issue Date** column shows the date on the supplier's invoice.")]
        [Guide("This date determines when the expense is recorded in your accounts and affects due date calculations.")]
        public DateTime[] GetIssueDate(PurchaseInvoice[] rows)
        {
            return rows.Select(x => x.IssueDate).ToArray();
        }

        [Guid("7de3c1a6-e645-4e79-b130-aa73cd249748")]
        [Guide("The **Due Date** column indicates when payment is due to the supplier.")]
        [Guide("This helps you manage cash flow and avoid late payment penalties.")]
        [Guide("Invoices past this date will show as overdue.")]
        public DateTime[] GetDueDate(PurchaseInvoice[] rows)
        {
            return rows.Select(x => x.GetDueDate()).ToArray();
        }

        [Default]
        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("2487abb0-630d-423d-91f9-5b90376cbc6d")]
        [Guide("The **Reference** column contains the supplier's invoice number.")]
        [Guide("This reference helps you match payments to invoices and resolve any queries with suppliers.")]
        public string[] GetReference(PurchaseInvoice[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Guid("9f33806e-efaf-4431-b8be-576cb3e48f32")]
        [Guide("The **Purchase Order** column shows which order this invoice fulfills.")]
        [Guide("This helps you verify that invoiced amounts match what was ordered and approved.")]
        public string[] GetPurchaseOrder(PurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<PurchaseOrder>(x.PurchaseOrder)?.GetName()).ToArray();
        }

        [Default]
        [Guid("6fb3ac93-65ec-4b35-9cc0-d0a6727f276e")]
        [Guide("The **Supplier** column displays which vendor sent this invoice.")]
        [Guide("The supplier name links to their full record where you can see all transactions and current balance owed.")]
        public string[] GetSupplier(PurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }        

        [Default]
        [Guid("262f1b23-e602-47b1-898c-3053d2e82d15")]
        [Guide("The **Description** column provides a summary of what this invoice covers.")]
        [Guide("This helps you quickly understand the nature of the expense without viewing the full invoice details.")]
        public string[] GetDescription(PurchaseInvoice[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("e5b462a2-00fd-4786-8178-8691ee6b5ed6")]
        [Guide("The **Project** column shows which projects incurred expenses on this invoice.")]
        [Guide("Since projects are assigned per line item, one invoice can include expenses for multiple projects.")]
        [Guide("This helps you track project costs and profitability.")]
        public string[] GetProject(PurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Project != null).Select(x => x.Project.Name).Distinct())).ToArray();
        }

        [Guid("1c3831b3-437e-48c2-bf4f-e8dcfbbdbc3e")]
        [Guide("The **Closed Invoice** column indicates whether this invoice has been marked as closed.")]
        [Guide("Closed invoices are excluded from certain reports and cannot be edited without reopening.")]
        public bool[] GetClosedInvoice(PurchaseInvoice[] rows)
        {
            return rows.Select(x => x.ClosedInvoice).ToArray();
        }

        [Guid("632db19b-dbf5-4fdb-9652-2815231ba0a7")]
        [Guide("The **Withholding Tax** column shows tax deducted from this invoice payment.")]
        [Guide("*Withholding tax* is typically deducted at source and remitted to tax authorities on the supplier's behalf.")]
        [Guide("This amount reduces what you need to pay the supplier directly.")]
        public Tuple<decimal, Currency>[] GetWithholdingTax(PurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.GeneralLedgerAccount is ManagerServer.Model.BalanceSheetWithholdingTaxPayableAccount)?.GetReversedTransactionAmountWithCurrency()).ToArray();
        }

        [Right, Sum]
        [Guid("cdfad6db-faa4-42c6-9f5c-74cc338940e2")]
        [Guide("The **Discount** column shows the total discount amount applied to this invoice.")]
        [Guide("Discounts can be line-item specific or apply to the entire invoice.")]
        [Guide("This reduces the total amount you owe to the supplier.")]
        public Tuple<decimal, Currency>[] GetDiscount(PurchaseInvoice[] rows)
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
        [Guid("09213f02-c78e-4c28-85af-44bb08668328")]
        [Guide("The **Invoice Amount** column displays the total invoice amount including all line items, taxes, and adjustments.")]
        [Guide("This is the full amount the supplier expects to be paid.")]
        [Guide("For foreign currency invoices, both the original and *base currency* amounts are shown.")]
        public Tuple<decimal, Currency>[] GetInvoiceAmount(PurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        private Dictionary<PurchaseInvoice, Balance> getBalances = null;
        public Dictionary<PurchaseInvoice, Balance> GetBalances(PurchaseInvoice[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<BaseCurrency>();
                var suppliers = rows.Where(x => x.Supplier.HasValue).Select(x => x.Supplier.Value).Distinct().ToArray();
                var purchaseInvoices = new HashSet<Guid>(rows.Select(x => x.Key));
                var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchPurchaseInvoices(suppliers);
                var balances = generalLedger.Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.PurchaseInvoice != null && purchaseInvoices.Contains(x.PurchaseInvoice.Key)).GroupBy(x => x.PurchaseInvoice.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount) * -1m);

                var output = new Dictionary<PurchaseInvoice, Balance>();
                foreach (var e in rows)
                {
                    var currency = database.SingleOrDefault<ManagerServer.Model.Currency>(database.SingleOrDefault<ManagerServer.Model.Supplier>(e.Supplier)?.Currency) as ManagerServer.Model.Currency ?? baseCurrency;

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
                        BalanceDue = new Tuple<decimal, Currency, BusinessTemplate>(invoiceBalance, currency, new PurchaseInvoiceTransactions() { Business = Business, PurchaseInvoice = e.Key, Referrer = referrer }),
                        Status = status,
                        DaysOverdue = daysOverdue,
                        DaysToDueDate = daysToDueDate
                    });
                }
                getBalances = output;
            }
            return getBalances;
        }

        [Right, Sum, WarnIfNegative]
        [Guid("85f6469d-69df-42f6-9ffb-95c0abd47014"), Default]
        [Guide("The **Balance Due** column shows the remaining amount you still owe on this invoice.")]
        [Guide("This balance decreases as you make payments to the supplier.")]
        [Guide("Click the amount to see all payments and credits applied to this invoice.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetBalanceDue(PurchaseInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].BalanceDue).ToArray();
        }

        [Center]
        [Guid("3ac2656f-6a16-4da3-9c33-b94552ae1470")]
        [Guide("The **Days To Due Date** column shows how many days remain until this invoice payment is due.")]
        [Guide("This countdown helps you plan cash flow and avoid late payments.")]
        [Guide("When this reaches zero, the invoice is due for payment today.")]
        public int?[] GetDaysToDueDate(PurchaseInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].DaysToDueDate).ToArray();
        }

        [Center]
        [Guid("6f294f59-9cc9-472a-b9ef-376593400a27")]
        [Guide("The **Days Overdue** column shows how many days have passed since the invoice due date.")]
        [Guide("Overdue invoices may incur late payment fees or damage supplier relationships.")]
        [Guide("Use this to prioritize which overdue invoices to pay first.")]
        public int?[] GetDaysOverdue(PurchaseInvoice[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].DaysOverdue).ToArray();
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("ce280d7b-0486-4aff-a223-9926a3a662f5"), Default]
        [Guide("The **Status** column shows the current payment status of this invoice at a glance.")]
        [Guide("Green indicates fully paid, yellow means payment is coming due, and red signals overdue.")]
        [Guide("This visual indicator helps you quickly identify invoices needing attention.")]
        public BalanceStatus[] GetStatus(PurchaseInvoice[] rows)
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
            if (!Supplier.HasValue && !PurchaseOrder.HasValue)
            {
                using (A(href: new PurchaseInvoiceLines() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.PurchaseInvoices + " - " + Strings.Lines);
            }
            base.OnFooterEndSection(context);
        }
    }
}

using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Customers))]
    [Guid("356ecb8d-36c6-4be5-b7a5-862050af55fb")]
    [Guide("The `Customers` tab is where you manage all your business relationships with people and organizations who buy from you.")]
    [Guide("This central hub lets you track essential customer information including contact details, addresses, financial balances, and transaction history.")]
    [Guide("From here, you can monitor outstanding invoices, view payment status, track deliveries, and manage credit limits for each customer.")]
    [TabScreenshot("fa-users-class", nameof(Strings.Customers))]
    [Header("Getting Started")]
    [Guide("To add a new customer, click the `New Customer` button.")]
    [HeroButtonScreenshot(nameof(Strings.Customers), nameof(Strings.NewCustomer))]
    [LinkGuide("For more information, see:", typeof(CustomerForm))]
    [Header("Understanding Customers")]
    [Guide("A customer is any individual, business, or organization that purchases goods or services from your business.")]
    [Guide("When you create a customer record, Manager automatically tracks their `Accounts Receivable` balance, which represents money they owe you.")]
    [Guide("You don't need to create a customer record for every sale. Cash sales paid immediately can be processed without creating a customer.")]
    [Guide("Customer records are most useful when you need to track credit sales, issue statements, or maintain ongoing business relationships.")]
    [Header("Setting Up Starting Balances")]
    [Guide("New customers always start with a zero balance. If you're migrating from another accounting system and the customer has outstanding invoices, you'll need to enter them separately.")]
    [Guide("To set up existing customer balances from your previous system:")]
    [Guide("• Enter each unpaid invoice individually under the `Sales Invoices` tab to enable accurate customer statements")]
    [Guide("• Cash-basis accounting users: invoices will appear in reports only when paid")]
    [Guide("• For credit balances (overpayments), create a credit note under the `Credit Notes` tab")]
    [Header("Customizing the Display")]
    [Guide("The `Customers` tab features several columns.")]
    [Columns]
    [Guide("Click the `Edit Columns` button to customize the visibility of columns.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithEditColumns<>))]
    [Header("Advanced Features")]
    [Guide("The `Advanced Queries` feature provides powerful ways to analyze and organize your customer data.")]
    [Guide("For example, if you track `Billable Time`, you can quickly find customers with uninvoiced work:")]
    [AdvancedQuery(select: new[] { nameof(Strings.Name), nameof(Strings.Uninvoiced) }, where: new[] { nameof(Strings.Uninvoiced), nameof(Strings.IsNotEmpty), null })]
    [Guide("This is just one example. You can create queries to find overdue accounts, analyze sales by customer, identify your top clients, and much more.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithAdvancedQueries))]
    internal sealed class Customers : NakedObjectsWithAutomaticRows<Customer>
    {
        [WarnIfNotUnique]
        [Guid("2a7b0329-aa8b-4e37-afe9-a1eb2dc530c3")]
        [Guide("The `Code` column shows the unique identifier or reference code assigned to each customer.")]
        [Guide("Customer codes help you quickly identify customers and can be used for sorting or searching.")]
        public string[] GetCode(ManagerServer.Model.Customer[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("8781dba6-3fb4-4158-8410-da6a1fffb5aa")]
        [Guide("The `Name` column displays the customer's full name or business name.")]
        [Guide("This is how the customer will appear on invoices, statements, and reports.")]
        public string[] GetName(ManagerServer.Model.Customer[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("94f8cd83-ee23-4c7e-b9ab-f8b1886c76f7")]
        [Guide("The `Email Address` column shows the primary email address for customer communication.")]
        [Guide("This email is used when sending invoices, statements, and other documents directly from Manager.")]
        public string[] GetEmailAddress(ManagerServer.Model.Customer[] rows)
        {
            return rows.Select(x => x.Email).ToArray();
        }

        [Guid("9ac64db3-8d0a-4018-b5f7-b34b6f693c27")]
        [Guide("The `Control Account` column indicates which control account tracks this customer's balance.")]
        [Guide("By default, all customers use the standard `Accounts Receivable` control account.")]
        [Guide("You can create custom control accounts under `Settings` → `Control Accounts` to separate different types of customers for reporting purposes.")]
        [LinkGuide("For more information, see:", typeof(Settings.ControlAccounts.ControlAccounts))]
        public string[] GetControlAccount(ManagerServer.Model.Customer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForCustomers>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetAccountsReceivableAccount>()).GetName()).ToArray();
        }

        [Guid("daf3c40b-e67f-48b1-97f7-fd3171873b54")]
        [Guide("The `Division` column shows which division this customer belongs to in your organizational structure.")]
        [Guide("Divisions help you track performance and generate reports for different parts of your business.")]
        [LinkGuide("For more information, see:", typeof(Settings.Divisions.Divisions))]
        public string[] GetDivision(ManagerServer.Model.Customer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.Name).ToArray();
        }

        [Guid("75020127-1426-4884-a27f-8dae1c596065")]
        [Guide("The `Billing Address` column contains the address where invoices and billing correspondence should be sent.")]
        [Guide("This address appears on sales invoices and customer statements.")]
        public string[] GetBillingAddress(ManagerServer.Model.Customer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.BillingAddress).ToArray();
        }

        [Guid("10072177-36e8-4219-b12d-0f254535fe7e")]
        [Guide("The `Delivery Address` column shows where goods should be shipped or services delivered.")]
        [Guide("If different from the billing address, this ensures deliveries reach the correct location.")]
        public string[] GetDeliveryAddress(ManagerServer.Model.Customer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.DeliveryAddress).ToArray();
        }

        [Center]
        [Guid("15308402-d5a4-4928-bd3e-55eb4f718206")]
        [Guide("The `Receipts` column shows how many payment receipts have been recorded for this customer.")]
        [Guide("Click the number to view all receipts and see payment history for this customer.")]
        [LinkGuide("For more information, see:", typeof(Receipts.Receipts))]
        public Tuple<int, BusinessTemplate>[] GetReceipts(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.Receipt>().Where(x => x.PaidBy == ManagerServer.Model.Enums.PayerPayeeType.Customer && x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new Receipts.Receipts() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("c5b77b0c-d7c7-4b84-bcd2-2eb7b9fab276")]
        [Guide("The `Payments` column shows the number of payments made to this customer.")]
        [Guide("These are typically refunds, overpayment returns, or other payments you've made to the customer.")]
        [Guide("Click the number to see all payment transactions for this customer.")]
        [LinkGuide("For more information, see:", typeof(Payments.Payments))]
        public Tuple<int, BusinessTemplate>[] GetPayments(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.Payment>().Where(x => x.Payee == ManagerServer.Model.Enums.PayerPayeeType.Customer && x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new Payments.Payments() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("7d2c0957-2e8a-47c2-9dff-37d9a4e49cf2")]
        [Guide("The `Sales Quotes` column shows how many quotes you've prepared for this customer.")]
        [Guide("Click the number to view all quotes, including their status and whether they've been converted to orders.")]
        public Tuple<int, BusinessTemplate>[] GetSalesQuotes(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.SalesQuote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new SalesQuotes.SalesQuotes() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("a73bf3ed-ec50-49e2-a914-f2a92c3c585c")]
        [Guide("The `Sales Orders` column indicates how many confirmed orders are recorded for this customer.")]
        [Guide("Click the number to see all orders, including pending and completed ones.")]
        public Tuple<int, BusinessTemplate>[] GetSalesOrders(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.SalesOrder>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new SalesOrders.SalesOrders() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("523f0ad9-b8c1-472b-93ce-9bc70e533f80")]
        [Guide("The `Sales Invoices` column shows the total number of invoices issued to this customer.")]
        [Guide("Click the number to view all invoices, see payment status, and track outstanding amounts.")]
        public Tuple<int, BusinessTemplate>[] GetSalesInvoices(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new SalesInvoices.SalesInvoices() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("df547bf0-a9d0-4621-8e3c-6bfc3cd87ffe")]
        [Guide("The `Credit Notes` column indicates how many credit notes have been issued to this customer.")]
        [Guide("Credit notes reduce the amount owed and are used for returns, allowances, or corrections.")]
        [Guide("Click the number to view all credit note details.")]
        public Tuple<int, BusinessTemplate>[] GetCreditNotes(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.CreditNote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new CreditNotes.CreditNotes() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("348fd8c8-eb30-414b-aa2c-dca4f2258005")]
        [Guide("The `Delivery Notes` column shows how many delivery notes document shipments to this customer.")]
        [Guide("Click the number to see all deliveries, including what was shipped and when.")]
        public Tuple<int, BusinessTemplate>[] GetDeliveryNotes(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.DeliveryNote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).GroupBy(x => x.Customer.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new DeliveryNotes.DeliveryNotes() { Business = Business, Customer = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Right]
        [Guid("c503a0c0-084d-4820-82f6-52c61cd14d9c")]
        [Guide("The `Qty to Deliver` column displays the total quantity of items sold but not yet delivered to this customer.")]
        [Guide("This helps you track pending deliveries and manage your fulfillment obligations.")]
        [Guide("Click the number to see a detailed breakdown by inventory item.")]
        [LinkGuide("For more information, see:", typeof(CustomersQtyToDeliver))]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToDeliver(ManagerServer.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var output = new List<Tuple<decimal, BusinessTemplate>>();
            var database = ApplicationData.Businesses.Get(Business);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));

            var transactions = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            transactions.AddRange(database.OfType<DeliveryNote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<SalesInvoice>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<CreditNote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<InventoryItemStartingBalance>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));

            var balances = transactions
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Customer != null)
                .GroupBy(x => x.Customer.Key)
                .ToDictionary(x => x.Key, x => x.GroupBy(x => x.InventoryItem).Select(x => x.Sum(y => y.QtyToDeliver)).ToArray());

            foreach (var e in rows)
            {
                var total = balances.TryGetValue(e.Key, out decimal[] amounts) ? amounts.Sum() : 0m;

                output.Add(new Tuple<decimal, BusinessTemplate>(total, new CustomersQtyToDeliver() { Business = Business, Customer = e.Key, Referrer = referrer }));
            }

            return output.ToArray();
        }

        /*
        [Right]
        [Guid("4d373239-5413-4b21-88b2-f5b1beea18f2")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToInvoice(Manager.Model.Customer[] rows)
        {
            var referrer = this.ToUrl();
            var output = new List<Tuple<decimal, BusinessTemplate>>();
            var database = Manager.ApplicationData.Businesses.Get(FileID);
            var customers = new HashSet<Guid>(rows.Select(x => x.Key));

            var transactions = new List<Manager.Query.GeneralLedger.GeneralLedgerTransaction>();
            transactions.AddRange(database.OfType<DeliveryNote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<SalesInvoice>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<CreditNote>().Where(x => x.Customer.HasValue && customers.Contains(x.Customer.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));

            var balances = transactions
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Customer != null)
                .GroupBy(x => x.Customer.Key)
                .ToDictionary(x => x.Key, x => x.GroupBy(x => x.InventoryItem).Select(x => x.Sum(y => y.QtyToDeliver)).ToArray());

            foreach (var e in rows)
            {
                var total = balances.TryGetValue(e.Key, out decimal[] amounts) ? amounts.Where(x => x < 0m).Sum() : 0m;

                output.Add(new Tuple<decimal, BusinessTemplate>(total*-1m, new CustomerQtyToInvoice() { FileID = FileID, Customer = e.Key, Referrer = referrer }));
            }

            return output.ToArray();
        }
        */

        private Dictionary<Customer, CustomerBalance> getBalances = null;
        public Dictionary<Customer, CustomerBalance> GetBalances(ManagerServer.Model.Customer[] rows)
        {
            if (getBalances == null)
            {
                var customers = new HashSet<Guid>(rows.Select(x => x.Key));
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
                var balances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Customer != null).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable || x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.WithholdingTaxReceivable || x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount || x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.BillableTimeUnbilled).Where(x => x.Customer != null).GroupBy(x => x.Customer.Key).Where(x => customers.Contains(x.Key)).ToDictionary(x => x.Key, x => x.ToArray());
                var output = new Dictionary<Customer, CustomerBalance>();
                foreach (var e in rows)
                {
                    decimal? availableCredit = null;
                    if (e.CreditLimit > 0m) availableCredit = e.CreditLimit;

                    var currency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(e.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                    var customerBalance = new CustomerBalance();
                    var link = new CustomerTransactions() { Business = Business, Customer = e.Key, Referrer = referrer };
                    var link2 = new WithholdingTaxReceivable() { Business = Business, Customer = e.Key, Referrer = referrer };
                    var link3 = new NewSalesInvoiceForm() { Business = Business, Customer = e.Key, Referrer = referrer };
                    if (balances.TryGetValue(e.Key, out GeneralLedgerTransaction[] transactions))
                    {
                        var accountsReceivable = transactions.Where(x => x.GeneralLedgerAccount.IsAccountsReceivable).Select(x => x.AccountAmount).SafeSum();
                        var withholdingTaxReceivable = transactions.Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.WithholdingTaxReceivable).Select(x => x.AccountAmount).SafeSum();
                        var billableTime = transactions.Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.BillableTimeUnbilled).Select(x => x.AccountAmount).SafeSum();
                        var billableExpenses = transactions.Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount).Select(x => x.AccountAmount).SafeSum();
                        customerBalance.AccountsReceivable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(accountsReceivable, currency, link);
                        customerBalance.WithholdingTaxReceivable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(withholdingTaxReceivable, currency, link2);
                        if (accountsReceivable > 0m) customerBalance.Status = CustomerStatus.Unpaid;
                        if (accountsReceivable < 0m) customerBalance.Status = CustomerStatus.Overpaid;

                        if (availableCredit.HasValue) availableCredit -= accountsReceivable;
                        if (availableCredit.HasValue) customerBalance.AvailableCredit = new Tuple<decimal, Currency>(availableCredit.Value, currency);

                        if (billableTime != 0m || billableExpenses != 0m)
                        {
                            customerBalance.Uninvoiced = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(billableTime + billableExpenses, currency, link3);
                        }
                    }
                    else
                    {
                        customerBalance.AccountsReceivable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(0m, currency, link);
                        customerBalance.WithholdingTaxReceivable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(0m, baseCurrency, link2);
                        if (availableCredit.HasValue) customerBalance.AvailableCredit = new Tuple<decimal, Currency>(availableCredit.Value, currency);
                    }
                    output.Add(e, customerBalance);
                }
                getBalances = output;
            }
            return getBalances;
        }

        [Right, Sum]
        [Guid("24bcc2c0-c010-40d7-9970-08b2d26b0a50")]
        [Guide("The `Uninvoiced` column shows the total value of billable work and expenses not yet billed to this customer.")]
        [Guide("This includes both `Billable Time` and `Billable Expenses` that are ready for invoicing.")]
        [Guide("Click the amount to create a new invoice for these unbilled items.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetUninvoiced(ManagerServer.Model.Customer[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].Uninvoiced).ToArray();
        }

        [Right, Sum]
        [Guid("5bb46491-8e3c-47be-b0e7-b08154aefd25"), Bold, Default]
        [Guide("The `Accounts Receivable` column shows the current balance this customer owes your business.")]
        [Guide("This balance increases when you issue sales invoices and decreases when you receive payments or issue credit notes.")]
        [Guide("Click the balance to see all transactions that make up this amount.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetAccountsReceivable(ManagerServer.Model.Customer[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].AccountsReceivable).ToArray();
        }

        [Right, Sum]
        [Guid("5cd4e359-a25c-4b34-8815-e0850a803248")]
        [Guide("The `Withholding Tax Receivable` column tracks tax amounts that customers have withheld from their payments to you.")]
        [Guide("In some jurisdictions, customers are required to withhold tax and remit it directly to tax authorities.")]
        [Guide("This amount represents tax credits you can claim once the customer pays the tax authority.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetWithholdingTaxReceivable(ManagerServer.Model.Customer[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].WithholdingTaxReceivable).ToArray();
        }

        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("f5a98b00-0fca-4463-9013-c0a5fd4c220a"), Default]
        [Guide("The `Status` column provides a quick visual indicator of the customer's payment status:")]
        [Guide("• `Paid` — The customer has no outstanding balance")]
        [Guide("• `Unpaid` — The customer owes money on one or more invoices")]
        [Guide("• `Overpaid` — The customer has a credit balance (paid more than owed)")]
        public CustomerStatus[] GetStatus(ManagerServer.Model.Customer[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].Status).ToArray();
        }

        [Right, Sum]
        [Guid("9ebd4341-177e-48b5-b81b-ac830aee5973")]
        [Guide("The `Available Credit` column shows how much more this customer can purchase on credit before reaching their limit.")]
        [Guide("This is calculated by subtracting the current `Accounts Receivable` balance from the customer's credit limit.")]
        [Guide("Set credit limits when editing a customer to help manage credit risk.")]
        public Tuple<decimal, Currency>[] GetAvailableCredit(ManagerServer.Model.Customer[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].AvailableCredit).ToArray();
        }

        public sealed class CustomerBalance
        {
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> Uninvoiced;
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> AccountsReceivable;
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> WithholdingTaxReceivable;
            public CustomerStatus Status;
            public Tuple<decimal, Currency> AvailableCredit;
        }

        public enum CustomerStatus
        {
            [ManagerServer.Model.Attributes.Success] Paid,
            [ManagerServer.Model.Attributes.Danger] Unpaid,
            [ManagerServer.Model.Attributes.Warning] Overpaid
        }
    }
}

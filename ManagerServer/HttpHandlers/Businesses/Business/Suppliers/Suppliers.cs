using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.HttpHandlers.Businesses.Business.Customers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Suppliers
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Suppliers))]
    [Guid("5b878a27-79b4-442a-b67a-c339bcb29b21")]
    [Guide("The `Suppliers` tab is where you manage all vendors and suppliers who provide goods or services to your business.")]
    [Guide("Here you can track supplier information, monitor what you owe them, and maintain a complete history of your purchases.")]
    [Header("Overview")]
    [Guide("Suppliers are essential for tracking your business relationships with vendors. Each supplier entry maintains a complete record of your transactions, balances, and communication details.")]
    [Guide("The supplier list provides a comprehensive view of all your vendors, their current balances, and quick access to related transactions.")]
    [TabScreenshot("fa-city", nameof(Strings.Suppliers))]
    [Header("Getting Started")]
    [Guide("To create a new supplier, click the `New Supplier` button.")]
    [HeroButtonScreenshot(nameof(Strings.Suppliers), nameof(Strings.NewSupplier))]
    [Header("Understanding Suppliers")]
    [Guide("A supplier is any individual, business, or organization from whom you purchase goods or services.")]
    [Guide("When you create a supplier record, Manager automatically tracks their balance in `Accounts Payable`, which represents money you owe them.")]
    [Guide("You don't need to create a supplier record for every purchase. Cash purchases paid immediately can be processed without creating a supplier.")]
    [Guide("Supplier records are most useful when you buy on credit, need to track purchase history, or maintain ongoing vendor relationships.")]
    [Header("Setting Up Starting Balances")]
    [Guide("New suppliers always start with a zero balance. If you're migrating from another accounting system and owe money to existing suppliers, you'll need to enter their unpaid invoices.")]
    [Guide("To set up existing supplier balances, enter each unpaid invoice individually under the `Purchase Invoices` tab. This ensures accurate supplier statements and payment tracking from day one.")]
    [Header("Customizing the Display")]
    [Guide("The `Suppliers` tab displays information in columns that can be customized to show the data most relevant to your business.")]
    [Guide("Click the `Edit Columns` button to choose which columns to display and arrange them in your preferred order.")]
    [Columns]
    internal sealed class Suppliers : NakedObjectsWithAutomaticRows<ManagerServer.Model.Supplier>
    {
        [WarnIfNotUnique]
        [Guid("f042004e-7f0a-4af8-8d2e-542b51eb2c3d")]
        [Guide("The `Code` column displays the unique identifier assigned to each supplier.")]
        [Guide("Supplier codes help with quick identification and can be used for sorting or searching.")]
        public string[] GetCode(ManagerServer.Model.Supplier[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("444d3de6-8dd8-4b55-9eef-0d339915c691")]
        [Guide("The `Name` column shows the supplier's business name or individual name.")]
        [Guide("This name appears on purchase orders, payment records, and supplier reports.")]
        public string[] GetName(ManagerServer.Model.Supplier[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("ae5edb2a-44cf-48b3-8dfa-bea5017267bf")]
        [Guide("The `Email Address` column contains the primary email for supplier communication.")]
        [Guide("This email is used when sending purchase orders, remittance advices, and other correspondence.")]
        public string[] GetEmailAddress(ManagerServer.Model.Supplier[] rows)
        {
            return rows.Select(x => x.Email).ToArray();
        }

        [Guid("f2f5beae-0cdc-42a3-a296-778326b89f26")]
        [Guide("The `Control Account` column indicates which control account tracks this supplier's balance.")]
        [Guide("By default, all suppliers use the standard `Accounts Payable` control account.")]
        [Guide("You can create custom control accounts under `Settings` → `Control Accounts` to separate different types of suppliers for reporting purposes.")]
        public string[] GetControlAccount(ManagerServer.Model.Supplier[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForSuppliers>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetAccountsPayableAccount>()).GetName()).ToArray();
        }

        [Guid("6d10c92a-9834-4b71-9d07-e69370bdff56")]
        [Guide("The `Division` column shows which division this supplier is associated with in your organizational structure.")]
        [Guide("Divisions help you track expenses and generate reports for different parts of your business.")]
        public string[] GetDivision(ManagerServer.Model.Supplier[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.Name).ToArray();
        }

        [Guid("4a0b22d0-455b-4492-908a-aafb76639b81")]
        [Guide("The `Address` column contains the supplier's business address.")]
        [Guide("This address appears on purchase orders and is used for correspondence.")]
        public string[] GetAddress(ManagerServer.Model.Supplier[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Address).ToArray();
        }

        [Center]
        [Guid("15ee2055-0281-4016-9f40-5fab8aca6b87")]
        [Guide("The `Receipts` column shows how many receipts you've recorded from this supplier.")]
        [Guide("These are typically refunds or other money received from the supplier.")]
        [Guide("Click the number to view all receipt transactions.")]
        public Tuple<int, BusinessTemplate>[] GetReceipts(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.Receipt>().Where(x => x.PaidBy == ManagerServer.Model.Enums.PayerPayeeType.Supplier && x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new Receipts.Receipts() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("220d3fce-f267-452c-81b4-376f5fce5d92")]
        [Guide("The `Payments` column displays the number of payments you've made to this supplier.")]
        [Guide("Click the number to see all payment transactions and remittance details.")]
        public Tuple<int, BusinessTemplate>[] GetPayments(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.Payment>().Where(x => x.Payee == ManagerServer.Model.Enums.PayerPayeeType.Supplier && x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new Payments.Payments() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("23b3896a-d6cc-4d6c-b0ad-00af3e52e35b")]
        [Guide("The `Purchase Quotes` column shows how many quotes you've received from this supplier.")]
        [Guide("Click the number to view all quotes, including their status and validity.")]
        public Tuple<int, BusinessTemplate>[] GetPurchaseQuotes(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.PurchaseQuote>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new PurchaseQuotes.PurchaseQuotes() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("2f14a715-ba6b-47f7-a472-ec1280273e03")]
        [Guide("The `Purchase Orders` column indicates how many orders you've placed with this supplier.")]
        [Guide("Click the number to see all orders, including pending and completed ones.")]
        public Tuple<int, BusinessTemplate>[] GetPurchaseOrders(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.PurchaseOrder>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new PurchaseOrders.PurchaseOrders() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("08df57c6-04b1-4637-9d4b-a5bd41d5eb62")]
        [Guide("The `Purchase Invoices` column shows the total number of invoices received from this supplier.")]
        [Guide("Click the number to view all invoices, check payment status, and see outstanding amounts.")]
        public Tuple<int, BusinessTemplate>[] GetPurchaseInvoices(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new PurchaseInvoices.PurchaseInvoices() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("4e9ef4f4-e8e4-4d8d-8755-2ae9656041f5")]
        [Guide("The `Debit Notes` column indicates how many debit notes have been issued to this supplier.")]
        [Guide("Debit notes reduce the amount you owe and are used for returns, allowances, or corrections.")]
        [Guide("Click the number to view all debit note details.")]
        public Tuple<int, BusinessTemplate>[] GetDebitNotes(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.DebitNote>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new DebitNotes.DebitNotes() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Center]
        [Guid("bf995958-a594-45c6-a30f-de9af0fbb503")]
        [Guide("The `Goods Receipts` column shows how many goods receipts document deliveries from this supplier.")]
        [Guide("Click the number to see all receipts, including what was received and when.")]
        public Tuple<int, BusinessTemplate>[] GetGoodsReceipts(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
            var totals = database.OfType<ManagerServer.Model.GoodsReceipt>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).GroupBy(x => x.Supplier.Value).ToDictionary(x => x.Key, x => x.Count());
            return rows.Select(x => totals.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new GoodsReceipts.GoodsReceipts() { Business = Business, Supplier = x.Key, Referrer = referrer }) : null).ToArray();
        }

        [Right]
        [Guid("2e6004f6-d5be-48b3-9770-3850c51c9b15")]
        [Guide("The `Qty to Receive` column displays the total quantity of items you've ordered but haven't received yet.")]
        [Guide("This helps you track pending deliveries and manage your inventory planning.")]
        [Guide("Click the number to see a detailed breakdown by purchase order and inventory item.")]
        [LinkGuide("For more information, see:", typeof(SuppliersQtyToReceive))]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToReceive(ManagerServer.Model.Supplier[] rows)
        {
            var referrer = this.ToUrl();
            var output = new List<Tuple<decimal, BusinessTemplate>>();
            var database = ApplicationData.Businesses.Get(Business);
            var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));

            var transactions = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            transactions.AddRange(database.OfType<PurchaseInvoice>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<GoodsReceipt>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<DebitNote>().Where(x => x.Supplier.HasValue && suppliers.Contains(x.Supplier.Value)).SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<InventoryItemStartingBalance>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));

            var balances = transactions
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.Supplier != null)
                .GroupBy(x => x.Supplier.Key)
                .ToDictionary(x => x.Key, x => x.GroupBy(x => x.InventoryItem).Select(x => x.Sum(y => y.QtyToReceive)).ToArray());

            foreach (var e in rows)
            {
                var total = balances.TryGetValue(e.Key, out decimal[] amounts) ? amounts.Where(x => x > 0m).Sum() : 0m;

                output.Add(new Tuple<decimal, BusinessTemplate>(total, new SuppliersQtyToReceive() { Business = Business, Supplier = e.Key, Referrer = referrer }));
            }

            return output.ToArray();
        }

        private Dictionary<Supplier, SupplierBalance> getBalance;
        public Dictionary<Supplier, SupplierBalance> GetBalance(ManagerServer.Model.Supplier[] rows)
        {
            if (getBalance == null)
            {
                var suppliers = new HashSet<Guid>(rows.Select(x => x.Key));
                var referrer = this.ToUrl();
                var database = ApplicationData.Businesses.Get(Business);
                var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
                var balances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsAccountsPayable || x.GeneralLedgerAccount.IsWithholdingTaxPayablePayable).Where(x => x.Supplier != null).GroupBy(x => x.Supplier.Key).Where(x => suppliers.Contains(x.Key)).ToDictionary(x => x.Key, x => x.ToArray());
                var output = new Dictionary<Supplier, SupplierBalance>();
                foreach (var e in rows)
                {
                    decimal? availableCredit = null;
                    if (e.CreditLimit > 0m) availableCredit = e.CreditLimit;

                    var currency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(e.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                    var supplierBalance = new SupplierBalance();
                    var link = new SupplierTransactions() { Business = Business, Supplier = e.Key, Referrer = referrer };
                    var link2 = new WithholdingTaxPayable() { Business = Business, Supplier = e.Key, Referrer = referrer };
                    if (balances.TryGetValue(e.Key, out GeneralLedgerTransaction[] transactions))
                    {
                        var accountsPayable = transactions.Where(x => x.GeneralLedgerAccount.IsAccountsPayable).Sum(x => x.AccountAmount) * -1m;
                        var withholdingTaxPayable = transactions.Where(x => x.GeneralLedgerAccount.IsWithholdingTaxPayablePayable).Sum(x => x.AccountAmount) * -1m;
                        supplierBalance.AccountsPayable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(accountsPayable, currency, link);
                        supplierBalance.WithholdingTaxPayable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(withholdingTaxPayable, baseCurrency, link2);
                        if (accountsPayable > 0m) supplierBalance.Status = SupplierStatus.Unpaid;
                        if (accountsPayable < 0m) supplierBalance.Status = SupplierStatus.Overpaid;

                        if (availableCredit.HasValue) availableCredit -= accountsPayable;
                        if (availableCredit.HasValue) supplierBalance.AvailableCredit = new Tuple<decimal, Currency>(availableCredit.Value, currency);
                    }
                    else
                    {
                        supplierBalance.AccountsPayable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(0m, currency, link);
                        supplierBalance.WithholdingTaxPayable = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(0m, baseCurrency, link2);
                        if (availableCredit.HasValue) supplierBalance.AvailableCredit = new Tuple<decimal, Currency>(availableCredit.Value, currency);
                    }
                    output.Add(e, supplierBalance);
                }
                getBalance = output;
            }
            return getBalance;
        }

        [Default]
        [Bold, Right, Sum]
        [Guid("a70ec366-d69e-4225-a366-108cec9e63b8")]
        [Guide("The `Accounts Payable` column shows how much you currently owe this supplier.")]
        [Guide("This balance increases when you receive purchase invoices and decreases when you make payments or receive debit notes.")]
        [Guide("Click the balance to see all transactions that make up this amount.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetAccountsPayable(ManagerServer.Model.Supplier[] rows)
        {
            var balances = GetBalance(rows);
            return rows.Select(x => balances[x].AccountsPayable).ToArray();
        }

        [Right, Bold, Sum]
        [Guid("e26ba119-b60a-407e-ab90-8ee6990610d0")]
        [Guide("The `Withholding Tax Payable` column tracks tax amounts you've withheld from payments to this supplier.")]
        [Guide("In some jurisdictions, you're required to withhold tax from supplier payments and remit it to tax authorities.")]
        [Guide("This amount represents tax you need to pay to the government on behalf of the supplier.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetWithholdingTaxPayable(ManagerServer.Model.Supplier[] rows)
        {
            var balances = GetBalance(rows);
            return rows.Select(x => balances[x].WithholdingTaxPayable).ToArray();
        }

        [Default]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("677b1e05-9765-4056-901c-3f294136a266")]
        [Guide("The `Status` column provides a quick visual indicator of the supplier's payment status:")]
        [Guide("• `Paid` — You have no outstanding balance with this supplier")]
        [Guide("• `Unpaid` — You owe money on one or more invoices")]
        [Guide("• `Overpaid` — You have a credit balance (paid more than owed)")]
        public SupplierStatus[] GetStatus(ManagerServer.Model.Supplier[] rows)
        {
            var balances = GetBalance(rows);
            return rows.Select(x => balances[x].Status).ToArray();
        }

        [Guid("6ef8563f-1bc2-4b21-97e1-e98f11aac4c5")]
        [Right, Sum]
        [Guide("The `Available Credit` column shows how much more you can purchase from this supplier before reaching your credit limit.")]
        [Guide("This is calculated by subtracting your current `Accounts Payable` balance from the credit limit the supplier has extended to you.")]
        [Guide("Set credit limits when editing a supplier to help manage cash flow and purchasing.")]
        public Tuple<decimal, Currency>[] GetAvailableCredit(ManagerServer.Model.Supplier[] rows)
        {
            var balances = GetBalance(rows);
            return rows.Select(x => balances[x].AvailableCredit).ToArray();
        }

        public sealed class SupplierBalance
        {
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> AccountsPayable;
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> WithholdingTaxPayable;
            public SupplierStatus Status;
            public Tuple<decimal, Currency> AvailableCredit;
        }

        public enum SupplierStatus
        {
            [ManagerServer.Model.Attributes.Success] Paid,
            [ManagerServer.Model.Attributes.Danger] Unpaid,
            [ManagerServer.Model.Attributes.Warning] Overpaid
        }
    }
}

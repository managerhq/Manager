using System.Linq;
using System.Collections.Generic;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseInvoices))]
    [Guid("af38ba6e-eca2-48d5-82fb-8d98ba1661f3")]
    [Title(nameof(Strings.RecurringPurchaseInvoices), nameof(Strings.Pending))]
    [Guide("Recurring purchase invoices allow you to automate the creation of regular supplier invoices that occur on a predictable schedule.")]
    [Guide("This feature is ideal for recurring expenses such as rent, subscriptions, monthly services, or any other regular payments to suppliers.")]
    [Guide("Each recurring purchase invoice will automatically generate a new *purchase invoice* based on the schedule you define, saving time and ensuring you never miss recording a regular expense.")]
    [Guide("To create a new recurring purchase invoice, click the **New Recurring Purchase Invoice** button. You can set the frequency, start date, and all the invoice details that will be used for each automatically generated invoice.")]
    [Columns]
    internal sealed class RecurringPurchaseInvoices : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringPurchaseInvoice>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("9710986f-46e4-4574-aeac-c2c0ba105209")]
        [Guide("Displays the date when the next *purchase invoice* will be automatically generated for each recurring transaction. This date is calculated based on the frequency and schedule you have configured.")]
        public DateTime?[] GetNextIssueDate(ManagerServer.Model.RecurringPurchaseInvoice[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("fef68a89-abe8-476f-8840-a3eba86cd378")]
        [Guide("Displays the *supplier* who will receive the automatically generated purchase invoices. This is the supplier you selected when setting up the recurring transaction.")]
        public string[] GetSupplier(ManagerServer.Model.RecurringPurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Supplier>(x.Supplier)?.Name).ToArray();
        }

        [Default]
        [Guid("80a4bbe9-7e65-4438-abb1-abc663d45832")]
        [Guide("Displays the description or reference for each recurring purchase invoice. This description helps you identify what each recurring transaction is for and will be copied to each automatically generated invoice.")]
        public string[] GetDescription(ManagerServer.Model.RecurringPurchaseInvoice[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("39b5c0ad-ceff-480d-b933-459cd118b55a")]
        [Guide("Displays the total amount for each recurring purchase invoice. If the supplier uses a *foreign currency*, the amount will be shown in that currency. This is the amount that will appear on each automatically generated invoice.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetAmount(ManagerServer.Model.RecurringPurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, ManagerServer.Model.Currency>>();
            foreach (var e in rows)
            {
                var purchaseInvoice = new ManagerServer.Model.PurchaseInvoice();
                Copy(e, purchaseInvoice);
                var balancingTransaction = purchaseInvoice.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetReversedTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}
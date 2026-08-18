using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [Guid("857F19D9-DEB8-410B-9BCB-9BFEDAD76A0D")]
    [Title(nameof(Strings.PurchaseInvoices))]
    [Guide("The **Pending Recurring Purchase Invoices** screen displays purchase invoices that are scheduled to be created automatically based on their *recurring settings*.")]
    [Guide("Use this screen to review upcoming purchase invoices before they are generated. You can see when each invoice is due, which supplier it is for, and the expected amount.")]
    [Guide("Purchase invoices shown here will be created automatically on their scheduled dates unless you modify or delete the *recurring transaction*.")]
    [Columns]
    internal sealed class PendingRecurringPurchaseInvoices : NakedObjectsOfPendingRecurringTransactions<RecurringPurchaseInvoice>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("2D8D38F6-4604-431A-9B5C-7CCC2D052ECA")]
        [Guide("Displays the scheduled date when this *recurring purchase invoice* will be automatically generated in the system.")]
        public DateTime?[] GetNextIssueDate(RecurringPurchaseInvoice[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("A521A18F-AD09-44F1-987F-C40BADF11CA0")]
        [Guide("Displays the supplier associated with each *recurring purchase invoice*. The supplier name will appear as shown in your supplier list.")]
        public string[] GetSupplier(RecurringPurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("1D3EEA22-AD96-4013-8093-FBF10DEC8A97")]
        [Guide("Shows the description or reference information for each *recurring purchase invoice*. This helps you identify the purpose of each scheduled invoice.")]
        public string[] GetDescription(RecurringPurchaseInvoice[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("4395A2C9-AB51-49E3-BE5E-934FF26CE586")]
        [Guide("Displays the total amount of each *recurring purchase invoice* in its designated currency. This is the amount that will be recorded when the invoice is created.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringPurchaseInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
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
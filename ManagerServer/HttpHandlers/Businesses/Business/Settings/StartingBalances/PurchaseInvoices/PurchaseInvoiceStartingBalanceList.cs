using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.PurchaseInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseInvoices))]
    [Guid("d7c1d505-1568-4528-80cf-4be23bdee739")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.PurchaseInvoices))]
    [Guide("This screen allows you to set up starting balances for purchase invoices that have been created under the **Purchase Invoices** tab.")]
    [Guide("Starting balances are used to record unpaid purchase invoices from your previous accounting system when transitioning to this software.")]
    [Guide("To create a new starting balance for a purchase invoice, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.PurchaseInvoices), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* form where you can enter the details of your unpaid purchase invoice.")]
    [LinkGuide("For more information, see:", typeof(PurchaseInvoiceStartingBalanceForm))]
    internal sealed class PurchaseInvoiceStartingBalanceList : NakedObjectsWithAutomaticRows<PurchaseInvoiceStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("bd01be47-3ba4-4749-8db9-8031d2db6574")]
        public NamedObject[] GetSupplier(PurchaseInvoiceStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(database.SingleOrDefault<PurchaseInvoice>(x.PurchaseInvoice)?.Supplier)).ToArray();
        }

        [Default]
        [Guid("8b285fe8-c627-4ab1-bbd6-c21e930e7c4a")]
        public NamedObject[] GetPurchaseInvoice(PurchaseInvoiceStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<PurchaseInvoice>(x.PurchaseInvoice)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("72fe9e63-843b-430a-a72f-8876ce71acf7")]
        public Tuple<decimal, Currency>[] GetPartialPayment(PurchaseInvoiceStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency()).ToArray();
        }
    }
}
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.SalesInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesInvoices))]
    [Guid("5aef588e-f46b-400d-abe1-ee3255ca1a6c")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.SalesInvoices))]
    [Guide("This screen allows you to set up starting balances for sales invoices that you have created under the **Sales Invoices** tab.")]
    [Guide("Starting balances are used to record unpaid sales invoices from your previous accounting system when you begin using this software.")]
    [Guide("To create a new starting balance for a sales invoice, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SalesInvoices), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for *Sales Invoice*.")]
    [LinkGuide("For more information, see:", typeof(SalesInvoiceStartingBalanceForm))]
    internal sealed class SalesInvoiceStartingBalanceList : NakedObjectsWithAutomaticRows<SalesInvoiceStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("675b5e56-9367-49ec-801f-0de5c142b266")]
        public NamedObject[] GetCustomer(SalesInvoiceStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(database.SingleOrDefault<SalesInvoice>(x.SalesInvoice)?.Customer)).ToArray();
        }

        [Default]
        [Guid("a4f60ccf-7db4-4d28-aabf-0653572422de")]
        public NamedObject[] GetSalesInvoice(SalesInvoiceStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<SalesInvoice>(x.SalesInvoice)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("fb3ed9ca-f6f7-419b-9904-f1ca3e24f018")]
        public Tuple<decimal, Currency>[] GetPartialPayment(SalesInvoiceStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency()).ToArray();
        }
    }
}
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("a1391657-278e-4443-bf91-97fd66d947eb")]
    [Title(nameof(Strings.WithholdingTaxReceipts))]
    [Guide("The **Withholding Tax Receipts** tab helps you keep track of all the *withholding tax receipts* you receive from customers. This feature is crucial for businesses to ensure they report taxes accurately by maintaining a record of the amounts withheld from payments.")]
    [TabScreenshot("fa-file-certificate", nameof(Strings.WithholdingTaxReceipts))]
    [Guide("To create a new withholding tax receipt, click the **New Withholding Tax Receipt** button.")]
    [HeroButtonScreenshot(nameof(Strings.WithholdingTaxReceipts), nameof(Strings.NewWithholdingTaxReceipt))]
    [Guide("The **Withholding Tax Receipts** tab includes several columns:")]
    [Columns]
    internal sealed class WithholdingTaxReceipts : NakedObjectsWithAutomaticRows<ManagerServer.Model.WithholdingTaxReceipt>
    {
        [Default]
        [WarnIfFutureDate]
        [Guid("ce324645-e9a6-4dfb-bbca-3f31df0b0e7e")]
        [Guide("The date when the *withholding tax receipt* was issued by the customer")]
        public DateTime[] GetDate(ManagerServer.Model.WithholdingTaxReceipt[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("6e273a57-28e8-4265-8d25-cef74f7b6b0c")]
        [Guide("The customer who issued the *withholding tax receipt*")]
        public string[] GetCustomer(ManagerServer.Model.WithholdingTaxReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Customer>(x.Customer)?.Name).ToArray();
        }

        [Default]
        [Guid("b6a2aa0a-5839-45d1-b992-f34758e930c4")]
        [Guide("Optional description or reference number for the *withholding tax receipt*")]
        public string[] GetDescription(ManagerServer.Model.WithholdingTaxReceipt[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right]
        [Sum]
        [Guid("748d75a6-841f-411e-ad5a-876f0dbcfbc8")]
        [Guide("The amount of tax withheld as shown on the receipt")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetAmount(ManagerServer.Model.WithholdingTaxReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetTransactionAmountWithCurrency() ?? new Tuple<decimal, ManagerServer.Model.Currency>(0m, null)).ToArray();
        }
    }
}

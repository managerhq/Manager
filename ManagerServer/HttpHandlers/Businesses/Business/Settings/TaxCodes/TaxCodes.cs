using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.TaxCodes
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("bf470161-dd90-4225-ac17-5a7fd0fb1716")]
    [Title(nameof(Strings.TaxCodes))]
    [Guide("`Tax Codes` define the tax rates that apply to your business transactions.")]
    [Guide("Each tax code represents a specific tax rate or combination of rates that you can apply to sales, purchases, and other transactions.")]
    [SettingsItemScreenshot("fa-percent", nameof(Strings.TaxCodes))]
    [Header("Creating Tax Codes")]
    [Guide("To create a new tax code, click the `New Tax Code` button.")]
    [HeroButtonScreenshot(nameof(Strings.TaxCodes), nameof(Strings.NewTaxCode))]
    [Guide("When setting up a tax code, you specify the tax rate and configure how it applies to different types of transactions.")]
    [LinkGuide("Learn more about tax code setup:", typeof(TaxCodeForm))]
    [Header("Managing Tax Codes")]
    [Guide("Tax codes appear in this list showing their name and usage.")]
    [Guide("The `Transactions` column shows how many transactions use each tax code. Click the number to view all transactions for that specific tax code.")]
    [Guide("You can apply tax codes to sales invoices, purchase invoices, receipts, payments, and most other transaction types where tax is relevant.")]
    internal sealed class TaxCodes : NakedObjectsWithAutomaticRows<ManagerServer.Model.TaxCode>
    {
        [Default]
        [Guid("e1a4da61-2bd9-4004-92a8-9708dfcf9f7d")]
        public string[] GetName(ManagerServer.Model.TaxCode[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Center, MinWidth]
        [Guid("753093c3-96ac-449b-bfa6-f1529094a37d")]
        public Tuple<int, BusinessTemplate>[] GetTransactions(ManagerServer.Model.TaxCode[] rows)
        {
            var referrer = this.ToUrl();
            var taxCodeTransactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.TaxCode != null).GroupBy(x => x.TaxCode.Key).ToDictionary(x => x.Key, x => x.GroupBy(y => y.Transaction).Count());
            return rows.Select(x => taxCodeTransactions.TryGetValue(x.Key, out int value) ? new Tuple<int, BusinessTemplate>(value, new TaxCodeTransactions() { Business = Business, TaxCode = x.Key, Referrer = referrer }) : null).ToArray();
        }
    }
}

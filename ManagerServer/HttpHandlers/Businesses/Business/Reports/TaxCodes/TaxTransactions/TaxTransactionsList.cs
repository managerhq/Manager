using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTransactions))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Tax Transactions* report displays a list of all tax-related transactions for a specified period.")]
    [Guide("This report helps you review and analyze tax amounts collected and paid, making it useful for tax compliance and reporting.")]
    [Guide("To create a new tax transactions report, go to the **Reports** tab, click **Tax Transactions**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxTransactions), name: nameof(Strings.NewReport))]
    internal sealed class TaxTransactionsList : PersistentObjectTable<ManagerServer.Model.TaxTransactions>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("2200150d-eefe-4f95-bf58-df429f1c4b0d")]
        public DateTime GetFromDate(ManagerServer.Model.TaxTransactions o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("c68865b1-51be-4638-bef4-ee33804db254")]
        public DateTime GetToDate(ManagerServer.Model.TaxTransactions o) => o.ToDate;

        [Guid("68bbba12-8fe9-4a4c-9c02-93fe055d7340")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxTransactions o) => o.AccountingMethod;

        [Guid("cbb7d920-4f2d-488e-8666-e91b050a996f")]
        public string GetDescription(ManagerServer.Model.TaxTransactions o) => o.Description;
    }
}
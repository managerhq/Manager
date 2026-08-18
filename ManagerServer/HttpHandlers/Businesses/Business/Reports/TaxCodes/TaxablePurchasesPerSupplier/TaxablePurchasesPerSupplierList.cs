using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    [Title(nameof(Strings.TaxablePurchasesPerSupplier))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Taxable Purchases per Supplier* report provides a detailed summary of taxable transactions with each supplier.")]
    [Guide("This report helps you analyze your tax obligations by showing the taxable purchases made from each of your suppliers during a specified period.")]
    [Guide("To create a new report, go to the **Reports** tab, click **Taxable Purchases per Supplier**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxablePurchasesPerSupplier), name: nameof(Strings.NewReport))]
    internal sealed class TaxablePurchasesPerSupplierList : PersistentObjectTable<ManagerServer.Model.TaxablePurchasesPerSupplier>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("bdd02dee-f376-44c2-9c2f-f1a8dcdfd91b")]
        public DateTime GetFromDate(ManagerServer.Model.TaxablePurchasesPerSupplier o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("42e5186c-17d8-4529-8403-3762f6e4925a")]
        public DateTime GetToDate(ManagerServer.Model.TaxablePurchasesPerSupplier o) => o.ToDate;

        [Guid("e4dc7683-ff3d-4999-985c-e52e6e1638c0")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxablePurchasesPerSupplier o) => o.AccountingMethod;

        [Guid("a318c81d-340c-429e-9421-cb5e21f7b9d2")]
        public string GetDescription(ManagerServer.Model.TaxablePurchasesPerSupplier o) => o.Description;
    }
}
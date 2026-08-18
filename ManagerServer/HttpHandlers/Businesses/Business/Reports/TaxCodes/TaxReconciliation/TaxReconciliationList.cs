using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    [Title(nameof(Strings.TaxReconciliation))]
    [Guide("*Tax Reconciliation* provides an overview of how tax amounts from tax codes and tax payments and refunds impact tax accounts.")]
    [Guide("To create a new tax reconciliation report, go to the **Reports** tab, click **Tax Reconciliation**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxReconciliation), name: nameof(Strings.NewReport))]
    internal sealed class TaxReconciliationList : PersistentObjectTable<ManagerServer.Model.TaxReconciliation>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b0142cb1-7d56-454e-aca3-8c989f12c460")]
        public DateTime? GetFromDate(ManagerServer.Model.TaxReconciliation o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("ab92086a-962b-4858-97fc-5b27f583e52a")]
        public DateTime? GetToDate(ManagerServer.Model.TaxReconciliation o) => o.Periods?[0].ToDate;

        [Guid("46fb8c88-cc68-49a6-a21d-60b94ac81b32")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxReconciliation o) => o.AccountingMethod;

        [Guid("af6fea91-9835-497c-b675-5b69a966aaa3")]
        public string GetDescription(ManagerServer.Model.TaxReconciliation o) => o.Description;
    }
}
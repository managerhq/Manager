using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    [Title(nameof(Strings.TaxAudit))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Tax Audit* report provides a comprehensive summary of how transactions have been categorized across tax codes for a specific period.")]
    [Guide("This report helps you verify that transactions have been assigned to the correct tax codes and can assist with tax compliance and filing.")]
    [Guide("To create a new tax audit report, go to the **Reports** tab, click **Tax Audit**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TaxAudit), name: nameof(Strings.NewReport))]
    internal sealed class TaxAuditList : PersistentObjectTable<ManagerServer.Model.TaxAudit>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("bedeb745-a901-43e9-bf04-3382f08400bb")]
        public DateTime GetFromDate(ManagerServer.Model.TaxAudit o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("ed8eb6d4-c158-4c5f-a103-c64c81ba6836")]
        public DateTime GetToDate(ManagerServer.Model.TaxAudit o) => o.ToDate;

        [Guid("6cb43780-a4b0-4714-909d-9887dd01321a")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TaxAudit o) => o.AccountingMethod;

        [Guid("e8519938-453e-40b9-aa8d-ab6817462c97")]
        public string GetDescription(ManagerServer.Model.TaxAudit o) => o.Description;
    }
}
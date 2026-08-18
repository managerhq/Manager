using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccountsSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`CapitalAccountsSummary` report provides a comprehensive overview of your capital accounts, detailing the current balance, transactions, and overall financial position.")]
    [Guide("To create a new `CapitalAccountsSummary`, go to `Reports` tab, click `CapitalAccountsSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.CapitalAccountsSummary), name: nameof(Strings.NewReport))]
    internal sealed class CapitalAccountsSummaryList : PersistentObjectTable<ManagerServer.Model.CapitalAccountsSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("feec9c2e-1692-49a0-b8ad-83db36749a77")]
        public DateTime GetFromDate(ManagerServer.Model.CapitalAccountsSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("e7d0b759-bb7c-4a4c-b6a3-c1fe927e126c")]
        public DateTime GetToDate(ManagerServer.Model.CapitalAccountsSummary o) => o.ToDate;

        [HideColumnIfAllEmpty]
        [Guid("ccd8f900-662e-4690-9729-92433cfd1a3c")]
        public string GetTitle(ManagerServer.Model.CapitalAccountsSummary o) => o.Title;

        [Guid("eb7f135b-c76f-47c9-b12c-a6b71db943d8")]
        public string GetDescription(ManagerServer.Model.CapitalAccountsSummary o) => o.Description;
    }
}
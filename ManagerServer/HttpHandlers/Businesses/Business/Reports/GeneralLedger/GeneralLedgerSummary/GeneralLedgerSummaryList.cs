using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerSummary))]
    [NewButton(nameof(Strings.GeneralLedgerSummary))]
    [Guide("`GeneralLedgerSummary` provides a concise overview of all financial transactions recorded in the general ledger, offering a snapshot of your business's financial performance and position over a specified period.")]
    [Guide("To create a new `GeneralLedgerSummary`, go to `Reports` tab, click `GeneralLedgerSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.GeneralLedgerSummary), name: nameof(Strings.NewReport))]
    internal sealed class GeneralLedgerSummaryList : PersistentObjectTable<ManagerServer.Model.GeneralLedgerSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("c4347b2d-9307-4219-941f-556a275b5295")]
        public DateTime GetFromDate(ManagerServer.Model.GeneralLedgerSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("0c316aff-1267-4a47-9bfa-1a5d47a68f3b")]
        public DateTime GetToDate(ManagerServer.Model.GeneralLedgerSummary o) => o.ToDate;

        [Guid("e42bd544-f722-409e-a099-b4257043940a")]
        public string GetDescription(ManagerServer.Model.GeneralLedgerSummary o) => o.Description;
    }
}
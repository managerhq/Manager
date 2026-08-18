using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerTransactions))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`GeneralLedgerTransactions` provides a detailed overview of all financial activities recorded in your general ledger, offering a comprehensive snapshot of your business's transaction history.")]
    [Guide("To create a new `GeneralLedgerTransactions`, go to `Reports` tab, click `GeneralLedgerTransactions`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.GeneralLedgerTransactions), name: nameof(Strings.NewReport))]
    internal sealed class GeneralLedgerTransactionsList : PersistentObjectTable<ManagerServer.Model.GeneralLedgerTransactions>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b1422bc8-9386-42dd-956a-bf0f825950cc")]
        public DateTime GetFromDate(ManagerServer.Model.GeneralLedgerTransactions o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("5e93447c-050e-4d70-9d7e-8f042571048d")]
        public DateTime GetToDate(ManagerServer.Model.GeneralLedgerTransactions o) => o.ToDate;

        [Guid("c48eaacc-ff06-40b0-8b19-bcdfb32feef3")]
        public string GetDescription(ManagerServer.Model.GeneralLedgerTransactions o) => o.Description;
    }
}
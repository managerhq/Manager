using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationCalculationWorksheet), nameof(Strings.DepreciationEntries))]
    [Guide("Shows depreciation entries for a specific fixed asset within the date range.")]
    [Guide("Displays all transactions affecting accumulated depreciation for the selected asset.")]
    internal sealed class DepreciationCalculationWorksheetDepreciationEntries : TransactionViewer
    {
        [ProtoMember(1)] public DateTime FromDate;
        [ProtoMember(2)] public DateTime ToDate;
        [ProtoMember(3)] public Guid FixedAsset;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation)
                .Where(x => x.FixedAsset.Key == FixedAsset)
                .Where(x => x.Date >= FromDate)
                .Where(x => x.Date <= ToDate);
        }
    }
}
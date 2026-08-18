using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DivisionExceptionReport
{
    [ProtoContract]
    [Title(nameof(Strings.DivisionExceptionReport), nameof(Strings.Transactions))]
    [Guide("Shows transactions for accounts that should have divisions but don't.")]
    [Guide("Lists all transactions within the date range that are missing division assignments.")]
    internal sealed class DivisionExceptionReportTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid GeneralLedgerAccount;
        [ProtoMember(2)] public DateTime From;
        [ProtoMember(3)] public DateTime To;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.BaseAmount != 0m && x.GeneralLedgerAccount.Key == GeneralLedgerAccount && x.Division == null && x.Date >= From && x.Date <= To);
        }
    }
}
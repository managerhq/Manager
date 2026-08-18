using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerSummary))]
    [Guide("The General Ledger Summary form configures parameters for account balance reports.")]
    [Guide("Set date ranges to analyze general ledger account movements and balances.")]
    [Fields(typeof(ManagerServer.Model.GeneralLedgerSummary))]
    internal sealed class GeneralLedgerSummaryForm : NakedVueForm<ManagerServer.Model.GeneralLedgerSummary>
    {        
    }
}

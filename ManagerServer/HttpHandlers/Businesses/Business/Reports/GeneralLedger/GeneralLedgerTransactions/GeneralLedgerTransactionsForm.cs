using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerTransactions))]
    [Guide("The General Ledger Transactions form configures parameters for transaction reports.")]
    [Guide("Set date ranges and filters to view detailed general ledger entries.")]
    [Fields(typeof(ManagerServer.Model.GeneralLedgerTransactions))]
    internal sealed class GeneralLedgerTransactionsForm : NakedVueForm<ManagerServer.Model.GeneralLedgerTransactions>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    [Title(nameof(Strings.TrialBalance))]
    [Guide("The `TrialBalance` report lists all general ledger accounts with their debit and credit balances as of a specific date.")]
    [Guide("This fundamental accounting report verifies that total debits equal total credits, confirming the books are in balance.")]
    [Guide("Use the trial balance to review account balances, detect errors, and prepare financial statements.")]
    [Guide("Configure the report date and display options to show zero-balance accounts or filter by account groups.")]
    [Fields(typeof(ManagerServer.Model.TrialBalance))]
    internal sealed class TrialBalanceForm : NakedVueForm<ManagerServer.Model.TrialBalance>
    {
    }
}

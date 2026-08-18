using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerSummary))]
    [Guide("The General Ledger Summary shows account balances and movements.")]
    [Guide("It displays opening balances, debits, credits, and closing balances for all accounts.")]
    [LinkGuide("For more information see:", typeof(GeneralLedgerSummaryForm))]
    internal sealed class GeneralLedgerSummaryView : DefaultView<GetGeneralLedgerSummaryView>
    {
    }
}
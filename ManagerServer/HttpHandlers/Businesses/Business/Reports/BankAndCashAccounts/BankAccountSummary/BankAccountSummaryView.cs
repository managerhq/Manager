using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.BankAccountSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BankAccountSummary))]
    [Guide("The Bank Account Summary report shows cash flows in and out of a bank account.")]
    [Guide("It displays receipts, payments, and net changes in cash for specified periods.")]
    [LinkGuide("For more information see:", typeof(BankAccountSummaryForm))]
    internal sealed class BankAccountSummaryView : DefaultView<GetBankAccountSummaryView>
    {
    }
}
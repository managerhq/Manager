using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TrialBalance;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    [Title(nameof(Strings.TrialBalance))]
    [Guide("The Trial Balance report shows debit and credit balances for all accounts.")]
    [Guide("It verifies that total debits equal total credits, ensuring balanced books.")]
    [LinkGuide("For more information see:", typeof(TrialBalanceForm))]
    internal sealed class TrialBalanceView : DefaultView<GetTrialBalanceView>
    {
    }
}
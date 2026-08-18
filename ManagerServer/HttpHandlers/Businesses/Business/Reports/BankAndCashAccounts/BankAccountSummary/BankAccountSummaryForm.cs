using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BankAccountSummary))]
    [Guide("The Bank Account Summary report provides an overview of all bank account balances.")]
    [Guide("Configure the report parameters to view bank account movements and balances.")]
    [Fields(typeof(ManagerServer.Model.BankAccountSummary))]
    internal sealed class BankAccountSummaryForm : NakedVueForm<ManagerServer.Model.BankAccountSummary>
    {
    }
}

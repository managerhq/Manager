using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerSummary))]
    [Guide("The Customer Summary report provides an overview of all customer balances and activity.")]
    [Guide("Configure the report parameters to view customer transactions and outstanding amounts.")]
    [Fields(typeof(ManagerServer.Model.CustomerSummary))]
    internal sealed class CustomerSummaryForm : NakedVueForm<ManagerServer.Model.CustomerSummary>
    {
    }
}
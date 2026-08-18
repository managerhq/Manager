using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedReceivables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedReceivables), nameof(Strings.Edit))]
    [Guide("The `AgedReceivables` report analyzes outstanding customer invoices by how long they've been unpaid.")]
    [Guide("Customer balances are grouped into aging periods like current, 30 days, 60 days, and 90+ days overdue.")]
    [Guide("Use this report to prioritize collection efforts, identify problem accounts, and assess credit risk.")]
    [Guide("The aging analysis helps you estimate potential bad debts and make informed credit decisions.")]
    [Guide("Configure the report date, aging periods, and whether to include zero-balance customers.")]
    [Fields(typeof(ManagerServer.Model.AgedReceivables))]
    internal sealed class AgedReceivablesForm : NakedVueForm<ManagerServer.Model.AgedReceivables>
    {        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedPayables), nameof(Strings.Edit))]
    [Guide("The **Aged Payables** report analyzes outstanding supplier invoices by how long they have been unpaid.")]
    [Guide("This report groups supplier balances into aging periods such as current, 30 days, 60 days, and 90+ days overdue.")]
    [Header("Purpose and Benefits")]
    [Guide("Use this report to effectively manage your cash flow and prioritize which suppliers to pay first.")]
    [Guide("The aging analysis helps you maintain good supplier relationships by tracking payment deadlines.")]
    [Guide("By monitoring aged payables, you can avoid late payment fees and take advantage of early payment discounts when offered.")]
    [Header("Report Configuration")]
    [Guide("Configure the report by selecting the *report date* to determine which invoices are included in the analysis.")]
    [Guide("Adjust the *aging periods* to match your business needs and payment terms.")]
    [Guide("Choose whether to include suppliers with zero balances to see a complete list of all suppliers or only those with outstanding amounts.")]
    [Fields(typeof(ManagerServer.Model.AgedPayables))]
    internal sealed class AgedPayablesForm : NakedVueForm<ManagerServer.Model.AgedPayables>
    {        
    }
}

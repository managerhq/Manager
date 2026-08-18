using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement), nameof(Strings.Edit))]
    [Guide("The Cash Flow Statement form is used to configure report parameters.")]
    [Guide("Set the reporting period and method to analyze cash movements.")]
    [Fields(typeof(ManagerServer.Model.CashFlowStatement))]
    internal sealed class CashFlowStatementForm : NakedVueForm<ManagerServer.Model.CashFlowStatement>
    {
    }
}

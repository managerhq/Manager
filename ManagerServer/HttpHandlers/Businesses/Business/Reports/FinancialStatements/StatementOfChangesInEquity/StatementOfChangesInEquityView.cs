using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Model.Master;
using ManagerServer.Api.Businesses.Business.Reports.StatementOfChangesInEquity;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.StatementOfChangesInEquity
{
    [ProtoContract]
    [Title(nameof(Strings.StatementOfChangesInEquity))]
    [Guide("The Statement of Changes in Equity report shows movements in owner's equity.")]
    [Guide("It tracks capital contributions, drawings, and retained earnings changes.")]
    [LinkGuide("For more information see:", typeof(StatementOfChangesInEquityForm))]
    internal sealed class StatementOfChangesInEquityView : DefaultView<GetStatementOfChangesInEquityView>
    {
    }
}
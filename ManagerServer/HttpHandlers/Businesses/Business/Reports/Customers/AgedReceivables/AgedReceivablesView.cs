using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.AgedReceivables;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedReceivables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedReceivables))]
    [Guide("The Aged Receivables report shows outstanding customer invoices grouped by age.")]
    [Guide("It helps analyze customer payment patterns and manage collections.")]
    [LinkGuide("For more information see:", typeof(AgedReceivablesForm))]
    internal sealed class AgedReceivablesView : DefaultView<GetAgedReceivablesView>
    {
    }
}

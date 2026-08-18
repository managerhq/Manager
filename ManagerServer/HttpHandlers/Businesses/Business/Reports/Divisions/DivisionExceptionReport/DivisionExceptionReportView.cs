using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.DivisionExceptionReport;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DivisionExceptionReport
{
    [ProtoContract]
    [Title(nameof(Strings.DivisionExceptionReport))]
    [Guide("The Division Exception Report identifies profit/loss transactions missing divisions.")]
    [Guide("It helps ensure all transactions are properly allocated to divisions.")]
    [LinkGuide("For more information see:", typeof(DivisionExceptionReportForm))]
    internal sealed class DivisionExceptionReportView : DefaultView<GetDivisionExceptionReportView>
    {
    }
}
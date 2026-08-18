using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DivisionExceptionReport
{
    [ProtoContract]
    [Title(nameof(Strings.DivisionExceptionReport))]
    [Guide("The Division Exception Report form configures parameters for tracking division issues.")]
    [Guide("Set date ranges and divisions to identify transactions with division exceptions.")]
    [Fields(typeof(ManagerServer.Model.DivisionExceptionReport))]
    internal sealed class DivisionExceptionReportForm : NakedVueForm<ManagerServer.Model.DivisionExceptionReport>
    {
    }
}

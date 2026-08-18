using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReportTransformationReports
{
    [ProtoContract]
    [Title(nameof(Strings.ReportTransformation))]
    [Guide("The Report Transformation form configures parameters for custom report transformations.")]
    [Guide("Set options to apply transformation rules and generate derived financial reports.")]
    [Fields(typeof(ManagerServer.Model.ReportTransformationReport))]
    internal sealed class ReportTransformationReportForm : NakedVueForm<ManagerServer.Model.ReportTransformationReport>
    {
        protected override void OnSource(ReportTransformationReport form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.ReportTransformation2)
            {
                form.ReportTransformation = source.Key;
            }
        }
    }
}
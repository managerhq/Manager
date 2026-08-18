using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReportTransformationReports
{
    [ProtoContract]
    [Title(nameof(Strings.ReportTransformations))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("Report transformations allow you to generate customized reports based on predefined transformation rules.")]
    [Guide("This list displays all reports that have been generated using the selected *report transformation*, showing the date range covered by each report.")]
    [Guide("Click **New Report** to generate a new report using this transformation, or click any existing report to view or edit it.")]
    [Columns]
    internal sealed class ReportTransformationReportList : PersistentObjectTable<ManagerServer.Model.ReportTransformationReport>
    {
        [ProtoMember(1)] public Guid ReportTransformation;

        protected override ReportTransformationReport[] Filter(ReportTransformationReport[] rows)
        {
            return rows.Where(x => x.ReportTransformation == ReportTransformation).ToArray();
        }

        protected override BusinessTemplate GetEdit(ReportTransformationReport o, string referrer)
        {
            if (o != null)
            {
                return new ReportTransformationReportForm()
                {
                    Business = Business,
                    Key = o.Key,
                    Referrer = referrer
                };
            }
            else
            {
                return new ReportTransformationReportForm()
                {
                    Business = Business,
                    Source = ReportTransformation,
                    Referrer = referrer
                };
            }
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("c8fdcd7d-791a-4aa9-8e3f-aa96b7e8ec40")]
        public DateTime GetFromDate(ManagerServer.Model.ReportTransformationReport o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("29d37c3e-bb83-4bc5-b0d7-cfed9bb4d237")]
        public DateTime GetToDate(ManagerServer.Model.ReportTransformationReport o) => o.ToDate;

        [HideColumnIfAllEmpty]
        [Guid("607c9d8f-7753-486d-ac4e-510e9104ca38")]
        public ManagerServer.Model.Employee GetEmployee(ManagerServer.Model.ReportTransformationReport o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Employee>(o.Employee);

        [Guid("c74bde49-53b5-4321-b951-fa959112e618")]
        public string GetDescription(ManagerServer.Model.ReportTransformationReport o) => o.Description;
    }
}
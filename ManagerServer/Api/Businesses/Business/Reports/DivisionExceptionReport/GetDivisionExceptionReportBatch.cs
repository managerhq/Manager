using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.DivisionExceptionReport
{
    [ProtoContract]
    internal sealed class GetDivisionExceptionReportBatch : GetObjectBatchEndpoint<Model.DivisionExceptionReport, GetDivisionExceptionReport, PostDivisionExceptionReport, PutDivisionExceptionReport, DeleteDivisionExceptionReport>
    {
    }
}

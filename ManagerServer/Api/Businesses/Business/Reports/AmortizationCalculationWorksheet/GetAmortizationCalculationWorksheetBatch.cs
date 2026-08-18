using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.AmortizationCalculationWorksheet
{
    [ProtoContract]
    internal sealed class GetAmortizationCalculationWorksheetBatch : GetObjectBatchEndpoint<Model.AmortizationCalculationWorksheet, GetAmortizationCalculationWorksheet, PostAmortizationCalculationWorksheet, PutAmortizationCalculationWorksheet, DeleteAmortizationCalculationWorksheet>
    {
    }
}

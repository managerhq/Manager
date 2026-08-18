using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    internal sealed class GetDepreciationCalculationWorksheetBatch : GetObjectBatchEndpoint<Model.DepreciationCalculationWorksheet, GetDepreciationCalculationWorksheet, PostDepreciationCalculationWorksheet, PutDepreciationCalculationWorksheet, DeleteDepreciationCalculationWorksheet>
    {
    }
}

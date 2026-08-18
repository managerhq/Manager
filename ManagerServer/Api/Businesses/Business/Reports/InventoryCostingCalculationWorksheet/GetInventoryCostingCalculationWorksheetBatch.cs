using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    internal sealed class GetInventoryCostingCalculationWorksheetBatch : GetObjectBatchEndpoint<Model.InventoryCostingCalculationWorksheet, GetInventoryCostingCalculationWorksheet, PostInventoryCostingCalculationWorksheet, PutInventoryCostingCalculationWorksheet, DeleteInventoryCostingCalculationWorksheet>
    {
    }
}

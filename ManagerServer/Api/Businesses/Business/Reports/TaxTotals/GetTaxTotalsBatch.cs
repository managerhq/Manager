namespace ManagerServer.Api.Businesses.Business.Reports.TaxTotals
{
    [ProtoContract]
    internal sealed class GetTaxTotalsBatch : GetObjectBatchEndpoint<Model.TaxTotals, GetTaxTotals, PostTaxTotals, PutTaxTotals, DeleteTaxTotals>
    {
    }
}

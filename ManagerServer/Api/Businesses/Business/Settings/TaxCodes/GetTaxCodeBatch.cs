namespace ManagerServer.Api.Businesses.Business.Settings.TaxCodes
{
    [ProtoContract]
    internal sealed class GetTaxCodeBatch : GetObjectBatchEndpoint<Model.TaxCode, GetTaxCode, PostTaxCode, PutTaxCode, DeleteTaxCode>
    {
    }
}

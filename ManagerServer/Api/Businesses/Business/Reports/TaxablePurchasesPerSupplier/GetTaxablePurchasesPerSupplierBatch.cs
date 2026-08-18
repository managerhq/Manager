using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    internal sealed class GetTaxablePurchasesPerSupplierBatch : GetObjectBatchEndpoint<Model.TaxablePurchasesPerSupplier, GetTaxablePurchasesPerSupplier, PostTaxablePurchasesPerSupplier, PutTaxablePurchasesPerSupplier, DeleteTaxablePurchasesPerSupplier>
    {
    }
}

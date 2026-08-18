using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Suppliers
{
    [ProtoContract]
    internal sealed class GetSupplierBatch : GetObjectBatchEndpoint<Model.Supplier, GetSupplier, PostSupplier, PutSupplier, DeleteSupplier>
    {
    }
}

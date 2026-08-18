using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Customers
{
    [ProtoContract]
    internal sealed class GetCustomerBatch : GetObjectBatchEndpoint<Model.Customer, GetCustomer, PostCustomer, PutCustomer, DeleteCustomer>
    {
    }
}

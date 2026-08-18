using ManagerServer.Model;
using System.ComponentModel;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceBatch : GetObjectBatchEndpoint<Model.SalesInvoice, GetSalesInvoice, PostSalesInvoice, PutSalesInvoice, DeleteSalesInvoice>
    {
        [Description("Filter sales invoices by specific customer")]
        [ProtoMember(1)] public Guid? Customer { get; set; }

        public override SalesInvoice[] Filter(SalesInvoice[] objects)
        {
            if (Customer.HasValue) objects = objects.Where(x => x.Customer == Customer).ToArray();
            return objects;
        }
    }
}

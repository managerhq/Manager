namespace ManagerServer.Api.Businesses.Business.Settings.Footers.SalesOrders
{
    [ProtoContract]
    internal sealed class GetSalesOrderFooterBatch : GetObjectBatchEndpoint<Model.SalesOrderFooter, GetSalesOrderFooter, PostSalesOrderFooter, PutSalesOrderFooter, DeleteSalesOrderFooter>
    {
    }
}

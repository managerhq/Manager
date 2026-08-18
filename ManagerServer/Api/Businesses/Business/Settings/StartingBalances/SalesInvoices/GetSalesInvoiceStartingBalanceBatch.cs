namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.SalesInvoices
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceStartingBalanceBatch : GetObjectBatchEndpoint<Model.SalesInvoiceStartingBalance, GetSalesInvoiceStartingBalance, PostSalesInvoiceStartingBalance, PutSalesInvoiceStartingBalance, DeleteSalesInvoiceStartingBalance>
    {
    }
}

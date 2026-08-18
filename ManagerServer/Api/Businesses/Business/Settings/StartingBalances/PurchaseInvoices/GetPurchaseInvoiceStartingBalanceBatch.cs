namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.PurchaseInvoices
{
    [ProtoContract]
    internal sealed class GetPurchaseInvoiceStartingBalanceBatch : GetObjectBatchEndpoint<Model.PurchaseInvoiceStartingBalance, GetPurchaseInvoiceStartingBalance, PostPurchaseInvoiceStartingBalance, PutPurchaseInvoiceStartingBalance, DeletePurchaseInvoiceStartingBalance>
    {
    }
}

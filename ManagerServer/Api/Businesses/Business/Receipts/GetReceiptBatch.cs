using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Receipts
{
    [ProtoContract]
    internal sealed class GetReceiptBatch : GetObjectBatchEndpoint<Model.Receipt, GetReceipt, PostReceipt, PutReceipt, DeleteReceipt>
    {
        public Guid? BankOrCashAccount { get; set; }

        public override Receipt[] Filter(Receipt[] objects)
        {
            if (BankOrCashAccount.HasValue) objects = objects.Where(x => x.ReceivedIn == BankOrCashAccount.Value).ToArray();
            return objects;
        }
    }
}
using ManagerServer.Model;
using System;
using System.ComponentModel;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Receipts
{
    [ProtoContract]
    internal sealed class GetReceiptBatch : GetObjectBatchEndpoint<Model.Receipt, GetReceipt, PostReceipt, PutReceipt, DeleteReceipt>
    {
        public Guid? BankOrCashAccount { get; set; }
        [Description("Only items dated on or after this date")]
        public DateTime? FromDate { get; set; }

        public override Receipt[] Filter(Receipt[] objects)
        {
            if (BankOrCashAccount.HasValue) objects = objects.Where(x => x.ReceivedIn == BankOrCashAccount.Value).ToArray();
            if (FromDate.HasValue) objects = objects.Where(x => x.Date >= FromDate.Value.Date).ToArray();
            return objects;
        }
    }
}
using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Payments
{
    [ProtoContract]
    internal sealed class GetPaymentBatch : GetObjectBatchEndpoint<Model.Payment, GetPayment, PostPayment, PutPayment, DeletePayment>
    {
        public Guid? BankOrCashAccount { get; set; }

        public override Payment[] Filter(Payment[] objects)
        {
            if (BankOrCashAccount.HasValue) objects = objects.Where(x => x.PaidFrom == BankOrCashAccount.Value).ToArray();
            return objects;
        }
    }
}

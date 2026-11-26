using ManagerServer.Model;
using System;
using System.ComponentModel;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Payments
{
    [ProtoContract]
    internal sealed class GetPaymentBatch : GetObjectBatchEndpoint<Model.Payment, GetPayment, PostPayment, PutPayment, DeletePayment>
    {
        public Guid? BankOrCashAccount { get; set; }
        [Description("Only items dated on or after this date")]
        public DateTime? FromDate { get; set; }

        public override Payment[] Filter(Payment[] objects)
        {
            if (BankOrCashAccount.HasValue) objects = objects.Where(x => x.PaidFrom == BankOrCashAccount.Value).ToArray();
            if (FromDate.HasValue) objects = objects.Where(x => x.Date >= FromDate.Value.Date).ToArray();
            return objects;
        }
    }
}

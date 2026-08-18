using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.BillableTime
{
    [ProtoContract]
    internal sealed class GetBillableTimeBatch : GetObjectBatchEndpoint<Model.BillableTime, GetBillableTime, PostBillableTime, PutBillableTime, DeleteBillableTime>
    {
    }
}

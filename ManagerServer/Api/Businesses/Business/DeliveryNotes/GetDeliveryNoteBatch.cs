using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    internal sealed class GetDeliveryNoteBatch : GetObjectBatchEndpoint<Model.DeliveryNote, GetDeliveryNote, PostDeliveryNote, PutDeliveryNote, DeleteDeliveryNote>
    {
    }
}

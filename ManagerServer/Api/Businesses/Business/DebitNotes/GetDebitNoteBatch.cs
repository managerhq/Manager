using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.DebitNotes
{
    [ProtoContract]
    internal sealed class GetDebitNoteBatch : GetObjectBatchEndpoint<Model.DebitNote, GetDebitNote, PostDebitNote, PutDebitNote, DeleteDebitNote>
    {
    }
}

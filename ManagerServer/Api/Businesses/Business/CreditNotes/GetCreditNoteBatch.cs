using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.CreditNotes
{
    [ProtoContract]
    internal sealed class GetCreditNoteBatch : GetObjectBatchEndpoint<Model.CreditNote, GetCreditNote, PostCreditNote, PutCreditNote, DeleteCreditNote>
    {
    }
}

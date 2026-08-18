using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Attachments
{
    [ProtoContract]
    internal sealed class GetAttachmentBatch : GetObjectBatchEndpoint<Model.Attachment, GetAttachment, PostAttachment, PutAttachment, DeleteAttachment>
    {
    }
}

using ManagerServer.Attributes;
using ManagerServer.Globalization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.Attachments
{
    [ProtoContract]
    [Title(nameof(Strings.Attachment), nameof(Strings.Delete))]
    [Guide("This handler removes attachments from business objects.")]
    [Guide("It deletes both the attachment record and the associated blob data.")]
    internal sealed class RemoveAttachment : BusinessHandler
    {
        [ProtoMember(1)] public Guid Key;
        [ProtoMember(2)] public string Referrer;

        public override Task Post()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var attachment = database.SingleOrDefault<ManagerServer.Model.Attachment>(Key);
            if (attachment != null)
            {
                var allowed = attachment.Object.HasValue && database.SingleOrDefault(attachment.Object.Value) != null
                    ? DeletePermission.CanDelete(this, Business, attachment.Object.Value)
                    : this.GetCurrentUserPermissions(Business).FullAccess;
                if (!allowed)
                {
                    Response.StatusCode = 409; // 409 Conflict
                    return Response.WriteAsync("You don't have a permission to delete this attachment.");
                }

                ApplicationData.Businesses.Process(Business, Key, GetUserName());
                ApplicationData.Businesses.DeleteBlob(Business, Key);
            }

            Response.Redirect(Referrer);
            return Task.CompletedTask;
        }
    }
}
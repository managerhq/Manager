using ManagerServer.Attributes;
using ManagerServer.Globalization;
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
            ApplicationData.Businesses.Process(Business, Key, GetUserName());
            ApplicationData.Businesses.DeleteBlob(Business, Key);
            Response.Redirect(Referrer);
            return Task.CompletedTask;
        }
    }
}
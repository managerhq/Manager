using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using Microsoft.AspNetCore.StaticFiles;

namespace ManagerServer.HttpHandlers.Businesses.Business.Attachments
{
    [ProtoContract]
    [Title(nameof(Strings.Attachment), nameof(Strings.View))]
    [Guide("This handler retrieves and displays attachments.")]
    [Guide("It serves the attachment file to the browser for viewing or download.")]
    public sealed class ViewAttachment : BusinessHandler
    {
        [ProtoMember(1)] public Guid Key;

        public override async Task Get()
        {
            if (!ApplicationData.Businesses.Exists(Business))
            {
                Response.Redirect("/");
                return;
            }
            var attachment = ApplicationData.Businesses.Get(Business)?.SingleOrDefault<ManagerServer.Model.Attachment>(Key);
            if (attachment == null)
            {
                Response.Redirect("/");
                return;
            }

            //string contentType = null;
            var provider = new FileExtensionContentTypeProvider();
            provider.TryGetContentType(attachment.Name, out string contentType);

            if (attachment.Sha256 != null)
            {
                var stream = await ApplicationData.Storage.ReadAsync(attachment.Sha256);
                if (stream != null)
                {
                    Response.ContentType = contentType;
                    Response.Headers["Content-Disposition"] = "inline; filename*=UTF-8''" + Uri.EscapeDataString(attachment.Name);
                    await stream.CopyToAsync(Response.Body);
                    return;
                }
            }

            var buffer = ApplicationData.Businesses.GetBlob(Business, Key);
            if (buffer != null)
            {
                Response.ContentType = contentType;
                Response.Headers["Content-Disposition"] = "inline; filename*=UTF-8''" + Uri.EscapeDataString(attachment.Name);
                await Response.Body.WriteAsync(buffer, 0, buffer.Length);
                return;
            }

            Response.Redirect("/");
        }
    }
}

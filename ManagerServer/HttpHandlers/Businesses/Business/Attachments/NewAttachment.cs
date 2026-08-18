using System;
using System.IO;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Attachments
{
    [ProtoContract]
    [Title(nameof(Strings.NewAttachment))]
    [Guide("This handler processes new attachment uploads.")]
    [Guide("Files are uploaded and stored as attachments associated with business objects.")]
    internal sealed class NewAttachment : BusinessHandler
    {
        [ProtoMember(1)] public Guid Key;

        public override async Task Post()
        {
            var mediaType = MediaTypeHeaderValue.Parse(Request.ContentType);

            if (mediaType.Boundary.HasValue)
            {
                var boundary = HeaderUtilities.RemoveQuotes(mediaType.Boundary).Value;
                var reader = new MultipartReader(boundary, Request.Body);

                var section = await reader.ReadNextSectionAsync();
                if (section != null && ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var disposition) && disposition.IsFileDisposition())
                {
                    var file = section.AsFileSection();
                    var key = Guid.CreateVersion7();

                    var sha256 = await ApplicationData.Storage.WriteAsync(file.FileStream);                    

                    /*
                    if (ApplicationData.S3Blobs != null) // Remote
                    {
                        await ApplicationData.S3Blobs.WriteAsync(file.FileStream);
                    }
                    */

                    ApplicationData.Businesses.Process(Business, new ManagerServer.Model.Attachment() { Key = key, Object = Key, Name = file.FileName, Date = DateTime.UtcNow, Size = (int)Request.ContentLength, Sha256 = sha256 }, GetUserName());
                }
            }

            Response.Headers["HX-Refresh"] = "true";
        }
    }
}
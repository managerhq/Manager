using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal sealed class Image : BusinessHandler
    {
        public override async Task Get()
        {
            var key = Request.Query["key"];

            _ = Guid.TryParse(key, out Guid key2);

            if (key2 == Guid.Empty)
            {
                Response.StatusCode = 400;
                return;
            }

            if (!ApplicationData.Businesses.Exists(Business))
            {
                Response.StatusCode = 404;
                return;
            }

            var blob = ApplicationData.Businesses.GetBlob2(Business, key2);
            if (blob == null)
            {
                Response.StatusCode = 404;
                return;
            }

            Response.Headers.CacheControl = "public, max-age=31536000, immutable";
            Response.ContentType = blob.ContentType;
            await Response.BodyWriter.WriteAsync(blob.Content);
        }

        public override async Task Post()
        {
            var form = await Request.ReadFormAsync();
            var image = form.Files[0];

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);

            var blob = new ManagerServer.ApplicationData.Blob()
            {
                Name = image.FileName,
                ContentType = image.ContentType,
                Content = ms.ToArray()
            };

            if (!ApplicationData.Businesses.Exists(Business))
            {
                Response.StatusCode = 400;
                return;
            }

            var key = ApplicationData.Businesses.InsertBlob(Business, blob);

            using (Script())
            {
                Write($@"document.getElementById(""{image.Name}"").value = ""{key.ToString()}"";");
                Write($@"document.getElementById(""{image.Name}"").dispatchEvent(new Event('input'));");
            }
        }
    }
}

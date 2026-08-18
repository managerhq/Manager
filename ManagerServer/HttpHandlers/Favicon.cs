using System;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Favicon : HttpHandler
    {
        public override async Task Get()
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Manager.ico");

            if (!File.Exists(path))
            {
                Response.StatusCode = 404;
                return;
            }

            var dateFormat = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
            Response.Headers["Cache-Control"] = "public, max-age=31536000";
            Response.Headers["Date"] = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString(dateFormat, CultureInfo.InvariantCulture);

            Response.ContentType = "image/x-icon";
            using (var fs = File.OpenRead(path))
            {
                await fs.CopyToAsync(Response.Body);
            }
        }
    }
}
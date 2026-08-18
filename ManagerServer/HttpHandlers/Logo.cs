using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ManagerServer.Helpers;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Logo : HttpHandler
    {
        public override async Task Get()
        {
            using var s = await ApplicationData.Assets.OpenReadAsync("logo.png");
            if (s == null)
            {
                Response.StatusCode = 404;
                return;
            }

            Response.ContentType = "image/png";
            Response.Headers["Cache-Control"] = "public, max-age=31536000";

            await s.CopyToAsync(Response.Body);
        }
    }
}
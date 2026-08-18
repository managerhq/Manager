using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class DownloadContent : HttpHandler
    {
        public override async Task Post()
        {
            var form = await Request.ReadFormAsync();
            var filename = form["filename"].ToString();
            var content = form["content"];

            Response.ContentType = "text/plain; charset=utf-8";
            Response.Headers["Content-Disposition"] = "attachment; filename*=UTF-8''" + Uri.EscapeDataString((filename ?? string.Empty).Replace('/', '_'));
            await Response.WriteAsync(content);
        }
    }
}
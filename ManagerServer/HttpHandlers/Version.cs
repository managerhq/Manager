using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Version : HttpHandler
    {
        public override Task Get()
        {
            Response.ContentType = "text/plain";
            return Response.WriteAsync(typeof(Version).Assembly.GetName().Version.ToString());
        }
    }
}
using System.Threading.Tasks;
using System.IO;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    public sealed class DismissBanner : HttpHandler
    {
        public sealed class FormData
        {
            public string Location;
        }

        public override async Task Post()
        {
            var form = await Request.ReadFormAsync();

            Environment.SetEnvironmentVariable("MANAGER_BANNER", null);

            Response.StatusCode = 302;
            Response.Redirect(form[nameof(FormData.Location)]);
        }
    }
}

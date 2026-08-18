using ManagerServer.Endpoints;
using ManagerServer.Model;
using Microsoft.AspNetCore.Http;

namespace ManagerServer.Api.Businesses.Business
{
    [ProtoContract]
    [ProducesContent("text/html")]
    internal sealed class GetView : ViewEndpoint<IResult>
    {
        [InheritedProtoMember(500)] public Guid? Theme { get; set; }        

        public override IResult AuthorizedHandle()
        {
            var business = GetApplicationData().Businesses.Get(Business);

            var customTheme = business.SingleOrDefault<CustomTheme>(Theme);
            if (customTheme != null)
            {
                return Results.Content(customTheme.GetCompleteTheme(), "text/html; charset=utf-8");
            }            

            var stream = typeof(Program).Assembly.GetManifestResourceStream("ManagerServer.wwwroot.resources.themes.default.html");
            return Results.File(fileStream: stream, contentType: "text/html; charset=utf-8");
        }
    }
}

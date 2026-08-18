using ManagerServer.Endpoints;
using ManagerServer.Model;
using Microsoft.AspNetCore.Http;

namespace ManagerServer.Api.Businesses.Business
{
    [ProtoContract]
    [ProducesContent("text/html")]
    internal sealed class GetCustomButtonHtml : AuthorizedEndpoint<IResult>
    {
        [ProtoMember(1)] public Guid? Key { get; set; }

        public override IResult AuthorizedHandle()
        {
            var customButton = GetApplicationData().Businesses.Get(Business).SingleOrDefault<CustomButton>(Key);
            if (customButton.Source == Model.Enums.ExtensionSource.Inline)
            {
                return Results.Content(customButton?.Html ?? string.Empty, "text/html; charset=utf-8");
            }
            else
            {
                return Results.NoContent();
            }
        }
    }
}

using Microsoft.AspNetCore.Http;

namespace ManagerServer.Api.Businesses.Business
{
    [ProtoContract]
    internal sealed class GetBusinessLogo : AuthorizedEndpoint<IResult>
    {
        [ProtoMember(1)] public long? Timestamp { get; set; }

        public override IResult AuthorizedHandle()
        {
            var database = GetApplicationData().Businesses.Get(Business);
            if (database == null) return Results.NotFound();
            var businessDetails = database.Single<ManagerServer.Model.BusinessDetails>();
            var businessLogo = GetApplicationData().Businesses.GetImage(Business, businessDetails.Key);
            if (businessLogo == null) return Results.NotFound();
            if (businessLogo.Item1 == null || businessLogo.Item1.Length == 0) return Results.NotFound();

            return Results.File(businessLogo.Item1, contentType: businessLogo.Item2);
        }
    }
}

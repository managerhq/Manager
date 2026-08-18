using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api
{
    internal sealed record DefaultResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetDefault : AuthenticatedEndpoint<DefaultResource>
    {
        public override DefaultResource AuthenticatedHandle()
        {
            var links = new Dictionary<string, Link>
            {
                ["businesses"] = new Link(new Businesses.GetBusinesses().ToUrl()),
            };

            return new DefaultResource(Links: links);
        }
    }
}

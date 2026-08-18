using ManagerServer.Api.Businesses.Business.Settings.InventoryLocations.CustomInventoryLocations;
using ManagerServer.Api.Businesses.Business.Settings.InventoryLocations.DefaultInventoryLocation;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.InventoryLocations
{
    internal sealed record InventoryLocationsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetInventoryLocations : AuthorizedEndpoint<InventoryLocationsResource>
    {
        public override InventoryLocationsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["customInventoryLocations"] = new Link(new GetCustomInventoryLocationBatch { Business = Business }.ToUrl());
            links["defaultInventoryLocation"] = new Link(new GetDefaultInventoryLocation { Business = Business }.ToUrl());

            return new InventoryLocationsResource(links);
        }
    }
}

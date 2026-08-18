using ManagerServer.Api.Businesses.Business;
using ManagerServer.Authentication;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses
{
    internal sealed record BusinessItem(
        string Name,
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    internal sealed record BusinessesResource(
        [property: JsonPropertyName("_links")]   Dictionary<string, Link> Links,
        [property: JsonPropertyName("_actions")] Dictionary<string, Link> Actions,
        BusinessItem[] Businesses);

    [ProtoContract]
    internal sealed class GetBusinesses : AuthenticatedEndpoint<BusinessesResource>
    {
        public override BusinessesResource AuthenticatedHandle()
        {
            var user = Context.GetManagerUser();
            var allNames = GetApplicationData().Businesses.GetAll();

            // Mirrors AuthorizedEndpoint.ResolveUserPermissions: admins and the desktop
            // edition see every business, everyone else only their assigned ones.
            var visibleNames = Edition.IsDesktop || user.Type == UserType.Administrator
                ? allNames
                : allNames.Where(name => user.Businesses != null && user.Businesses.Contains(name));

            var items = visibleNames
                .Select(name => new BusinessItem(
                    Name: name,
                    Links: new Dictionary<string, Link>
                    {
                        ["tabs"] = new Link(new GetTabs { Business = name }.ToUrl()),
                    }))
                .ToArray();

            var links = Hyperlinks.ForCurrentDocument(this);
            var actions = new Dictionary<string, Link>
            {
                ["create"] = Hyperlinks.ForAction<PostBusiness>(),
            };

            return new BusinessesResource(
                Links: links,
                Actions: actions,
                Businesses: items);
        }
    }
}

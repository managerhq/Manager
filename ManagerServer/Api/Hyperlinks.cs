using ManagerServer.Endpoints;
using System.Collections.Generic;

namespace ManagerServer.Api
{
    internal sealed record Link(string Href);

    internal static class Hyperlinks
    {
        public static Dictionary<string, Link> ForCurrentDocument<T>(Endpoint<T> endpoint)
        {
            return new Dictionary<string, Link>
            {
                ["self"]        = new Link(endpoint.ToUrl()),
                ["describedBy"] = new Link($"/openapi/{Endpoint.ToKebabCase(endpoint.GetType())}.json"),
            };
        }

        public static Link ForAction<TAction>()
            => new Link($"/openapi/{Endpoint.ToKebabCase(typeof(TAction))}.json");
    }
}

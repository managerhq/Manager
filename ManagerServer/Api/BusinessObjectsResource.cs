using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api
{
    internal sealed record BusinessObjectsResource<T>(
        [property: JsonPropertyName("_links")]   Dictionary<string, Link> Links,
        [property: JsonPropertyName("_actions")] Dictionary<string, Link> Actions,
        Item<T>[] Items,
        // How many items match the request's filters, before Skip and PageSize.
        int Total);
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api
{
    internal sealed record Item<T>(
        Guid Key,
        [property: JsonPropertyName("item")]     T Value,
        [property: JsonPropertyName("_links")]   Dictionary<string, Link> Links,
        [property: JsonPropertyName("_actions")] Dictionary<string, Link> Actions);
}

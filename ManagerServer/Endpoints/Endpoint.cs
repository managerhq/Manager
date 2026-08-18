using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ManagerServer.Endpoints
{
    internal static class Endpoint
    {
        private static readonly string[] VerbPrefixes = ["Delete", "Patch", "Post", "Put", "Get", "Query"];

        public static string ToUrl(Type endpointType)
        {
            const string prefix = "/api4/";
            var name = endpointType.Name;
            var start = 0;
            foreach (var verb in VerbPrefixes)
            {
                if (name.Length > verb.Length
                    && name.StartsWith(verb, StringComparison.Ordinal)
                    && char.IsUpper(name[verb.Length]))
                {
                    start = verb.Length;
                    break;
                }
            }
            if (string.CompareOrdinal(name, start, "Default", 0, name.Length - start) == 0)
            {
                return "/api4";
            }
            var sb = new StringBuilder(prefix.Length + name.Length - start + 4);
            sb.Append(prefix);
            for (var i = start; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c) && sb.Length > prefix.Length) sb.Append('-');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }

        public static string ToKebabCase(Type endpointType)
        {
            var name = endpointType.Name;
            var sb = new StringBuilder(name.Length + 4);
            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c) && i > 0) sb.Append('-');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }
    }

    internal abstract class Endpoint<T>
    {
        [JsonIgnore]
        public HttpContext Context { get; set; }

        public virtual T Handle()
        {
            return default;
        }

        public virtual string ToUrl()
        {
            return Endpoint.ToUrl(GetType());
        }
    }
}
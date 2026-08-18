using System;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Helpers;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal sealed class Typeahead : BusinessHandler
    {
        [ProtoMember(1)] public Guid Type;
        [ProtoMember(2)] public string Field;

        public override Task Get()
        {
            Response.ContentType = "text/html";

            if (string.IsNullOrWhiteSpace(Business)) return Task.CompletedTask;

            var type = ManagerServer.Model.Object.GetTypeByGuid(Type);

            if (type == null) return Task.CompletedTask;

            var field = type.GetFieldOrProperty(Field);

            if (field?.GetCustomAttribute<ManagerServer.Model.Attributes.TypeaheadAttribute>() == null)
            {
                if (type == null) return Task.CompletedTask;
            }

            var typeAccessor = FastMember.TypeAccessor.Create(type);

            var items = ApplicationData.Businesses.Get(Business)
                .UnorderedOfType(type)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => typeAccessor[x, Field] as string)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct();

            var query = Request.Query["query"];

            if (!string.IsNullOrWhiteSpace(query)) items = items.Where(x => Search(x, query)).ToArray();

            foreach (var e in items.Take(10))
            {
                Option(value: e);
            }

            return Task.CompletedTask;
        }

        private bool Search(string value, string term)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (string.IsNullOrWhiteSpace(term)) return false;
            if (value == term) return false;
            return (value.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
        }
    }
}
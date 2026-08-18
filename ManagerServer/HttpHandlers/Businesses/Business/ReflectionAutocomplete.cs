using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ManagerServer.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal sealed class ReflectionAutocomplete : BusinessHandler
    {
        public override Task Get()
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (!ApplicationData.Businesses.Exists(Business))
            {
                return Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    more = false,
                    results = new object[0]
                }));
            }

            var term = Request.Query["Term"].ToString();
            var filter = Request.Query["Filter"].ToString();
            int.TryParse(Request.Query["Page"], out int page);

            var type = typeof(ManagerServer.Model.Object).Assembly
                .GetType(filter);

            var items = type.GetMembers()
                .Where(x => x.DeclaringType == type)
                .Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                .Where(x => !x.Name.StartsWith("Obsolete_"))
                .Select(x => new ManagerServer.Model.MemberInfo(x))
                .ToList();

            items = items.Where(x => !string.IsNullOrWhiteSpace(x.UniqueName)).ToList();
            if (!string.IsNullOrWhiteSpace(term)) items = items.Where(x => Search(x.UniqueName, term)).ToList();
            items = items.OrderBy(x => x.UniqueName).Skip((page - 1) * 10).ToList();

            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.Formatting = Formatting.Indented;

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                more = (items.Count < 10 ? false : true),
                results = items.Take(10).ToArray()
            }, jsonSettings);

            return Response.WriteAsync(json);
        }

        private static ManagerServer.Model.MemberInfo.FieldType GetValueType(MemberInfo memberInfo)
        {
            Type valueType = null;
            if (memberInfo is FieldInfo fieldInfo)
            {
                valueType = fieldInfo.FieldType;
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                valueType = propertyInfo.PropertyType;
            }
            else
            {
                return 0;
            }

            if (valueType == typeof(string)) return ManagerServer.Model.MemberInfo.FieldType.String;
            if (valueType == typeof(bool)) return ManagerServer.Model.MemberInfo.FieldType.Boolean;
            if (valueType == typeof(DateTime)) return ManagerServer.Model.MemberInfo.FieldType.Date;
            if (valueType == typeof(decimal)) return ManagerServer.Model.MemberInfo.FieldType.Decimal;
            if (valueType.IsSubclassOf(typeof(ManagerServer.Model.Object))) return ManagerServer.Model.MemberInfo.FieldType.Object;

            return 0;
        }

        private static bool HasMembers(Type type)
        {
            if (type == null) return false;
            return type.IsSubclassOf(typeof(ManagerServer.Model.Object));
        }

        private bool Search(string value, string term)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (string.IsNullOrWhiteSpace(term)) return false;
            return (value.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithJsonOutput : NakedObjectsWithObscureMode
    {
        [ProtoMember(200)] public bool JsonOutput;
        [ProtoMember(201), JsonProperty("fields")] public string Fields;

        protected override void InnerGet4(Context context)
        {
            var rows = context.Get<Array>();
            var columns = context.Get<Column[]>();

            if (JsonOutput)
            {
                var visibleFields = columns.Where(x => x.Visible && !string.IsNullOrWhiteSpace(x.Name) && x.CanEnsureCells(rows)).ToArray();
                if (!string.IsNullOrWhiteSpace(Fields))
                {
                    var fields = Fields.Split(',').Select(x => x.Trim()).ToArray();
                    visibleFields = columns.Where(x => fields.Contains(x.Name) && x.CanEnsureCells(rows)).ToArray();
                }

                foreach (var e in visibleFields)
                {
                    e.EnsureCells(rows);
                }

                var items = new List<JObject>();
                foreach (var e in rows)
                {
                    var item = new JObject();
                    if (e is ManagerServer.Model.Object o)
                    {
                        item["key"] = o.Key;
                    }
                    foreach (var e2 in visibleFields)
                    {
                        var key = ToCamelCase(e2.Name);
                        item[key] = e2.GetValueAsJToken(e);
                    }
                    items.Add(item);
                }

                var business = new JObject();
                business["name"] = Business;

                var json = new JObject();
                json["business"] = business;
                if (this is NakedObjectsWithPagination nakedObjectsWithPagination)
                {
                    json["skip"] = nakedObjectsWithPagination.Skip;
                    json["pageSize"] = nakedObjectsWithPagination.PageSize ?? 50;
                }
                json["totalRecords"] = context.Get<NakedObjectsWithPagination.Total>()?.Value ?? rows.Length;
                json[ToCamelCase(this.GetType().Name)] = new JArray(items);

                var output = json.ToString();

                Response.WriteAsync(output).GetAwaiter().GetResult();
                return;
            }

            base.InnerGet4(context);
        }

        private string ToCamelCase(string s)
        {
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
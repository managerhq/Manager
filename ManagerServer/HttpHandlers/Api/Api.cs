using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ManagerServer.HttpHandlers.Api
{
    [ProtoContract]
    internal sealed class Api : HttpHandler
    {
        private static string Utf8StringToUrlString(string value)
        {
            return Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(value)).TrimEnd(new char[] { '=' }).Replace('+', '-').Replace('/', '_');
        }

        public override Task Options()
        {
            return Task.CompletedTask;
        }

        public override Task Get()
        {
            if (!IsAuth()) return Task.CompletedTask;

            var path = Request.Path.Value.Substring(('/'+this.GetType().Name).Length);

            if (path == "/")
            {
                Response.Redirect(new Api().ToUrl());
                return Task.CompletedTask;
            }

            if (path.EndsWith(".json"))
            {
                if (path.Equals(".json"))
                {
                    Response.ContentType = "application/json";
                    return Response.WriteAsync(Serialize(ApplicationData.Businesses.GetAll().OrderBy(x => x).Select(x => new Business() { Key = Utf8StringToUrlString(x), Name = x }).ToArray()));
                }

                var parts = path.Substring(1).Split('/');

                if (parts.Length == 1)
                {
                    var businessName = System.Text.UTF8Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0].Split('.').First()));
                    if (!ApplicationData.Businesses.Exists(businessName)) return Task.CompletedTask;

                    return Response.WriteAsync(Serialize(ApplicationData.Businesses.Get(businessName).UnorderedOfType<ManagerServer.Model.Object>()
                        .GroupBy(x => x.GetType())
                        .Where(x => x.Key != typeof(ManagerServer.Model.Schema))
                        .Where(x => x.Key.Namespace == "ManagerServer.Model")
                        .Select(x => new Tuple<Type, int>(x.Key, x.Count()))
                        .OrderByDescending(x => x.Item2)
                        .Select(x => new Collection() { Key = ManagerServer.Model.Object.GetGuidByType(x.Item1).ToString(), Name = x.Item1.Name })
                        .ToArray()));
                }

                if (parts.Length > 1)
                {
                    var businessName = System.Text.UTF8Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0]));
                    if (!ApplicationData.Businesses.Exists(businessName)) return Task.CompletedTask;

                    var textKey = parts.Last().Split('.').First();
                    if (Guid.TryParse(parts.Last().Split('.').First(), out Guid key))
                    {
                        var type = ManagerServer.Model.Object.GetTypeByGuid(key);

                        if (type != null && type.GetCustomAttribute<ManagerServer.Model.Attributes.SingletonAttribute>() == null)
                        {
                            return Response.WriteAsync(Serialize(ApplicationData.Businesses.Get(businessName).UnorderedOfType<ManagerServer.Model.Object>()
                                .Where(x => x.GetType() == type)
                                .Where(x => x.Key != key)
                                .OrderByDescending(x => x.Timestamp)
                                .Select(x => new Item() { Key = x.Key.ToString(), Timestamp = x.Timestamp, Name = (x is ManagerServer.Model.NamedObject namedObject ? namedObject.GetName() : null) })
                                .ToArray()));
                        }
                        else
                        {
                            var o = ApplicationData.Businesses.Get(businessName).SingleOrDefault(key);
                            return Response.WriteAsync(Serialize(o));
                        }
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return Response.WriteAsync(Serialize(new Result() { Success = false, Error = "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)" }));
                    }
                }                                
            }
            else
            {
                string business = null;
                Guid? key = null;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var parts = path.Substring(1).Split('/');
                    if (parts.Length > 0) business = System.Text.Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0]));
                    if (parts.Length > 1 && Guid.TryParse(parts.Last(), out Guid result)) key = result;
                }

                using (Html())
                {
                    using (Head())
                    {
                        Write(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />");
                        Title("Manager API");
                        Link(rel: "shortcut icon", type: "image/x-icon", href: "favicon.ico");
                        Link(rel: "stylesheet", type: "text/css", href: "/resources/bootstrap5/css/bootstrap.css?" + typeof(Template).Assembly.GetName().Version.ToString());
                        using (Style())
                        {
                            Write("textarea { white-space: pre; overflow-wrap: normal; overflow-x: scroll; }");
                        }
                    }
                    using (Body())
                    {
                        using (Div(@class: "container"))
                        {
                            using (Ol(@class: "breadcrumb bg-light border p-3 mt-4"))
                            {
                                using (Li(@class: "breadcrumb-item"))
                                {
                                    if (!string.IsNullOrWhiteSpace(path)) using (A(href: new Api().ToUrl())) Write("Manager API");
                                    else Write("Manager API");
                                }

                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    var parts = path.Substring(1).Split('/');
                                    var url = new Api().ToUrl();
                                    for (int i = 0; i < parts.Length; i++)
                                    {
                                        url += '/' + parts[i];
                                        using (Li(@class: "breadcrumb-item"))
                                        {
                                            if (Guid.TryParse(parts[i], out Guid result))
                                            {
                                                var type = ManagerServer.Model.Object.GetTypeByGuid(result);
                                                if (type != null)
                                                {
                                                    if (i < parts.Length - 1) using (A(href: url)) Write(type.Name);
                                                    else Write(type.Name);
                                                }
                                                else
                                                {
                                                    if (i < parts.Length - 1) using (A(href: url)) Write(result.ToString());
                                                    else Write(result.ToString());
                                                }
                                            }
                                            else
                                            {
                                                var name = System.Text.Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[i]));
                                                if (i < parts.Length-1) using (A(href: url)) Write(name);
                                                else Write(name);
                                            }
                                        }
                                    }
                                }
                            }

                            using (Div(@class: "row"))
                            {
                                using (Div(@class: "col-6"))
                                {
                                    GetPlayground();
                                    if (key.HasValue)
                                    {
                                        var type = ManagerServer.Model.Object.GetTypeByGuid(key.Value);
                                        var o = ApplicationData.Businesses.Get(business).SingleOrDefault(key.Value);
                                        if (type != null)
                                        {
                                            if (type.GetCustomAttribute<ManagerServer.Model.Attributes.SingletonAttribute>() == null)
                                            {
                                                PostPlayground(type);
                                            }
                                            else
                                            {
                                                PutPlayground(o);
                                                PatchPlayground(o);
                                            }
                                        }
                                        else if (o != null)
                                        {
                                            PutPlayground(o);
                                            PatchPlayground(o);
                                            DeletePlayground();
                                        }
                                    }
                                }
                                using (Div(@class: "col-6"))
                                {
                                    Output();
                                }
                            }
                        }

                        using (Script())
                        {
                            Write(@"
                            window.console = {
                            log: function(str){
                                    var json = JSON.parse(str);
                                    if (Array.isArray(json)) {
                                        json.forEach(x => { if (x.Key) x.Key = '<a href='+window.location.href+'/'+x.Key+'>'+x.Key+'</a>' });
                                    }
                                    else {
                                        if (json.Key) json.Key = '<a href='+window.location.href+'/'+json.Key+'>'+json.Key+'</a>';
                                    }
                                    document.getElementById('output').innerHTML = JSON.stringify(json, null, 2);
                                    document.getElementById('output').classList.remove('d-none');
                                }
                            }");
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void GetPlayground()
        {
            using (Div(@class: "card mb-4"))
            {
                using (Div(@class: "card-header"))
                {
                    Write("Example");
                    using (Span(@class: "badge bg-secondary ms-2")) Write("GET");
                }
                using (Div(@class: "card-body"))
                {
                    using (H5(@class: "card-title")) Write("Javascript");
                    using (Pre(id: "input", @class: "p-2 bg-dark text-white font-monospace"))
                    {
                        var endpoint = Request.Path.ToString() + ".json";

                        Write("fetch('");
                        using (Script()) Write("document.write(window.location.href);");
                        Write(".json')");
                        Write(Environment.NewLine);
                        Write("\t.then(response => response.json())");
                        Write(Environment.NewLine);
                        Write("\t.then(data => console.log(JSON.stringify(data, null, 2)));");
                    }
                    using (Button(@class: "btn btn-secondary fw-bold", onclick: "document.getElementById('output').classList.add('d-none'); eval(document.getElementById('input').innerText)"))
                    {
                        Write("Get");
                    }
                }
            }

            using (Script())
            {
                Write("eval(document.getElementById('input').innerText);");
            }
        }

        private void PutPlayground(ManagerServer.Model.Object o)
        {
            using (Div(@class: "card mb-4"))
            {
                using (Div(@class: "card-header"))
                {
                    Write("Example");
                    using (Span(@class: "badge bg-success ms-2")) Write("PUT");
                }
                using (Div(@class: "card-body"))
                {
                    using (H5(@class: "card-title")) Write("Javascript");
                    Textarea(id: "put-input", text: Serialize(o, defaultValueHandlingIgnore: false), @class: "form-control", rows: 10);
                    using (Pre(id: "put-script", @class: "p-2 bg-dark text-white font-monospace"))
                    {
                        var endpoint = Request.Path.ToString() + ".json";

                        Write("fetch('");
                        using (Script()) Write("document.write(window.location.href);");
                        Write(".json', {");
                        Write(Environment.NewLine);
                        Write("\t\tmethod: 'PUT',");
                        Write(Environment.NewLine);
                        Write("\t\tbody: document.getElementById('put-input').value");
                        Write(Environment.NewLine);
                        Write("\t})");
                        Write(Environment.NewLine);
                        Write("\t.then(response => response.json())");
                        Write(Environment.NewLine);
                        Write("\t.then(data => console.log(JSON.stringify(data, null, 2)));");
                    }
                    using (Button(@class: "btn btn-success fw-bold", onclick: "document.getElementById('output').classList.add('d-none'); eval(document.getElementById('put-script').innerText)"))
                    {
                        Write("Put");
                    }
                }
            }
        }

        private void PatchPlayground(ManagerServer.Model.Object o)
        {
            using (Div(@class: "card mb-4"))
            {
                using (Div(@class: "card-header"))
                {
                    Write("Example");
                    using (Span(@class: "badge bg-warning ms-2")) Write("PATCH");
                }
                using (Div(@class: "card-body"))
                {
                    using (H5(@class: "card-title")) Write("Javascript");
                    Textarea(id: "patch-input", text: Serialize(o, defaultValueHandlingIgnore: false), @class: "form-control", rows: 5);
                    using (Pre(id: "patch-script", @class: "p-2 bg-dark text-white font-monospace"))
                    {
                        var endpoint = Request.Path.ToString() + ".json";

                        Write("fetch('");
                        using (Script()) Write("document.write(window.location.href);");
                        Write(".json', {");
                        Write(Environment.NewLine);
                        Write("\t\tmethod: 'PATCH',");
                        Write(Environment.NewLine);
                        Write("\t\tbody: document.getElementById('patch-input').value");
                        Write(Environment.NewLine);
                        Write("\t})");
                        Write(Environment.NewLine);
                        Write("\t.then(response => response.json())");
                        Write(Environment.NewLine);
                        Write("\t.then(data => console.log(JSON.stringify(data, null, 2)));");
                    }
                    using (Button(@class: "btn btn-warning fw-bold", onclick: "document.getElementById('output').classList.add('d-none'); eval(document.getElementById('patch-script').innerText)"))
                    {
                        Write("Patch");
                    }
                }
            }
        }

        private void PostPlayground(Type type)
        {
            using (Div(@class: "card mb-4"))
            {
                using (Div(@class: "card-header"))
                {
                    Write("Example");
                    using (Span(@class: "badge bg-primary ms-2")) Write("POST");
                }
                using (Div(@class: "card-body"))
                {
                    using (H5(@class: "card-title")) Write("Javascript");
                    Textarea(text: Serialize(Activator.CreateInstance(type), defaultValueHandlingIgnore: false), @class: "form-control form-control-sm font-monospace", rows: 10);
                    using (Pre(id: "put-input", @class: "p-2 bg-dark text-white font-monospace"))
                    {
                        var endpoint = Request.Path.ToString() + ".json";

                        Write("fetch('");
                        using (Script()) Write("document.write(window.location.href);");
                        Write(".json', {");
                        Write(Environment.NewLine);
                        Write("\t\tmethod: 'POST',");
                        Write(Environment.NewLine);
                        Write("\t\tbody: document.getElementsByTagName('textarea')[0].value");
                        Write(Environment.NewLine);
                        Write("\t})");
                        Write(Environment.NewLine);
                        Write("\t.then(response => response.json())");
                        Write(Environment.NewLine);
                        Write("\t.then(data => console.log(JSON.stringify(data, null, 2)));");
                    }
                    using (Button(@class: "btn btn-primary fw-bold", onclick: "document.getElementById('output').classList.add('d-none'); eval(document.getElementById('put-input').innerText)"))
                    {
                        Write("Post");
                    }
                }
            }
        }

        private void DeletePlayground()
        {
            using (Div(@class: "card mb-4"))
            {
                using (Div(@class: "card-header"))
                {
                    Write("Example");
                    using (Span(@class: "badge bg-danger ms-2")) Write("DELETE");
                }
                using (Div(@class: "card-body"))
                {
                    using (H5(@class: "card-title")) Write("Javascript");
                    using (Pre(id: "delete-input", @class: "p-2 bg-dark text-white font-monospace"))
                    {
                        var endpoint = Request.Path.ToString() + ".json";

                        Write($"fetch('http://localhost:8080{endpoint}', {{");
                        Write(Environment.NewLine);
                        Write("\t\tmethod: 'DELETE'");
                        Write(Environment.NewLine);
                        Write("\t})");
                        Write(Environment.NewLine);
                        Write("\t.then(response => response.json())");
                        Write(Environment.NewLine);
                        Write("\t.then(data => console.log(JSON.stringify(data, null, 2)));");
                    }
                    using (Button(@class: "btn btn-danger fw-bold", onclick: "document.getElementById('output').classList.add('d-none'); eval(document.getElementById('delete-input').innerText)"))
                    {
                        Write("Delete");
                    }
                }
            }
        }

        private void Output()
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    Write("Output");
                }
                using (Div(@class: "card-body"))
                {
                    using (Pre(id: "output", @class: "font-monospace"))
                    {
                    }
                }
            }
        }

        public override async Task Post()
        {
            if (!IsAuth()) return;

            try
            {
                var path = Request.Path.Value.Substring(('/' + this.GetType().Name).Length);
                var parts = path.Substring(1).Split('/');
                var guid2 = Guid.Parse(parts[1].Split('.').First());
                var type = ManagerServer.Model.Object.GetTypeByGuid(guid2);

                var json = string.Empty;
                using (var s = new System.IO.StreamReader(Request.Body)) json = await s.ReadToEndAsync();

                var value = (ManagerServer.Model.Object)Newtonsoft.Json.JsonConvert.DeserializeObject(json, ManagerServer.Model.Object.GetTypeByGuid(guid2));
                value.Key = Guid.CreateVersion7();

                var businessName = System.Text.UTF8Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0]));
                ApplicationData.Businesses.Process(businessName, value, GetUserName());
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Headers["Location"] = "/api/" + parts[0] + "/" + value.Key.ToString() + ".json";
                await Response.WriteAsync(Serialize(new Result() { Success = true, Key = value.Key.ToString() }));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                Response.ContentType = "application/json";
                await Response.WriteAsync(Serialize(new Result() { Success = false, Error = ex.Message }));
            }
        }

        public override async Task Put()
        {
            if (!IsAuth()) return;

            try
            {
                var path = Request.Path.Value.Substring(('/' + this.GetType().Name).Length);
                var parts = path.Substring(1).Split('/');
                var businessName = System.Text.UTF8Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0]));
                var guid2 = Guid.Parse(parts.Last().Split('.').First());

                ManagerServer.ApplicationData.Object prevObject = null;
                if (ApplicationData.Businesses.Exists(businessName))
                {
                    using (var c = ApplicationData.Businesses.SQLiteConnection(businessName))
                    {
                        prevObject = c.Get<ManagerServer.ApplicationData.Object>(guid2);
                    }
                }
                var prevType = ManagerServer.Model.Object.GetTypeByGuid(prevObject.ContentType);

                var json = string.Empty;
                using (var s = new System.IO.StreamReader(Request.Body)) json = await s.ReadToEndAsync();

                var newObject = (ManagerServer.Model.Object)Newtonsoft.Json.JsonConvert.DeserializeObject(json, prevType);
                newObject.Key = guid2;

                ApplicationData.Businesses.Process(businessName, newObject, GetUserName());
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                await Response.WriteAsync(Serialize(new Result() { Success = true }));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                Response.ContentType = "application/json";
                await Response.WriteAsync(Serialize(new Result() { Success = false, Error = ex.Message }));
            }
        }

        public override async Task Patch()
        {
            if (!IsAuth()) return;

            try
            {
                var path = Request.Path.Value.Substring(('/' + this.GetType().Name).Length);
                var parts = path.Substring(1).Split('/');
                var businessName = System.Text.UTF8Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0]));
                var guid2 = Guid.Parse(parts.Last().Split('.').First());

                var json = string.Empty;
                using (var s = new System.IO.StreamReader(Request.Body)) json = await s.ReadToEndAsync();

                var o = ApplicationData.Businesses.Get(businessName).SingleOrDefault(guid2);
                var o1 = JObject.Parse(Serialize(o));
                var o2 = JObject.Parse(json);
                o1.Merge(o2, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });

                var newObject = (ManagerServer.Model.Object)new JsonSerializer().Deserialize(new JTokenReader(o1), o.GetType());
                newObject.Key = guid2;

                ApplicationData.Businesses.Process(businessName, newObject, GetUserName());
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                await Response.WriteAsync(Serialize(new Result() { Success = true }));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                Response.ContentType = "application/json";
                await Response.WriteAsync(Serialize(new Result() { Success = false, Error = ex.Message }));
            }
        }

        public override Task Delete()
        {
            if (!IsAuth()) return Task.CompletedTask;
            try
            {
                var path = Request.Path.Value.Substring(("/"+this.GetType().Name).Length);
                var parts = path.Substring(1).Split('/');
                if (parts.Length > 1 && parts.Last().EndsWith(".json"))
                {
                    var businessName = System.Text.UTF8Encoding.UTF8.GetString(HttpFramework.Serialization.UrlStringToBytes(parts[0]));
                    var guid2 = Guid.Parse(parts.Last().Split('.').First());

                    ApplicationData.Businesses.Process(businessName, guid2, GetUserName());
                }
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                return Response.WriteAsync(Serialize(new Result() { Success = true }));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                Response.ContentType = "application/json";
                return Response.WriteAsync(Serialize(new Result() { Success = false, Error = ex.Message }));
            }
        }

        private string Serialize(object value, bool defaultValueHandlingIgnore = true)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, new Newtonsoft.Json.JsonSerializerSettings()
            {
                DefaultValueHandling = defaultValueHandlingIgnore ? Newtonsoft.Json.DefaultValueHandling.Ignore : DefaultValueHandling.Include,
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatString = "yyyy-MM-dd",
                Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() },
                ContractResolver = new DynamicContractResolver(!(value is ManagerServer.Model.Object))
            });
        }

        public sealed class DynamicContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            private bool includeKey;

            public DynamicContractResolver(bool includeKey)
            {
                this.includeKey = includeKey;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization).ToArray();
                properties = properties.Where(x => !x.PropertyName.StartsWith("Obsolete_")).ToArray();
                if (!includeKey) properties = properties.Where(x => x.PropertyName != "Key").ToArray();
                //else properties = properties.OrderByDescending(x => x.PropertyName == "Key").ToArray();
                properties = properties.Where(x => x.Writable).ToArray();
                foreach (var e in properties.Where(x => x.PropertyType == typeof(string))) e.ValueProvider = new EmptyStringToNullValueProvider(e.ValueProvider);
                return properties;
            }
        }

        private sealed class EmptyStringToNullValueProvider : IValueProvider
        {
            private readonly IValueProvider provider;

            public EmptyStringToNullValueProvider(IValueProvider provider)
            {
                this.provider = provider;
            }

            public object GetValue(object target)
            {
                var text = provider.GetValue(target) as string;
                if (string.IsNullOrWhiteSpace(text)) return null;
                return text;
            }

            public void SetValue(object target, object value)
            {
                provider.SetValue(target, value);
            }
        }

        private bool IsAuth()
        {
            if (string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
            {
                Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                Response.StatusCode = 401;
                return false;
            }
            var authorization = Request.Headers["Authorization"].ToString().Split(' ');
            if (authorization[0] != "Basic")
            {
                Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                Response.StatusCode = 401;
                return false;
            }

            var usernamePassword = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(authorization[1])).Split(':');
            var username = usernamePassword[0];
            var password = usernamePassword[1];

            if (string.IsNullOrWhiteSpace(password))
            {
                Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                Response.StatusCode = 401;
                return false;
            }

            var user = ApplicationData.Users.GetByUsernameAsync(username).GetAwaiter().GetResult();

            if (user == null)
            {
                Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                Response.StatusCode = 401;
                return false;
            }
            if (user.Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                Response.StatusCode = 401;
                return false;
            }
            if (!user.Verify(password, null))
            {
                Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                Response.StatusCode = 401;
                return false;
            }
            
            return true;
        }

        public sealed class Business
        {
            public string Key;
            public string Name;
        }

        public sealed class Collection
        {
            public string Key;
            public string Name;
            public int Count;
        }

        public sealed class Result
        {
            public bool? Success;
            public string Error;
            public string Key;
        }

        public sealed class Item
        {
            public string Key;
            public string Name;
            public long Timestamp;
        }
    }
}
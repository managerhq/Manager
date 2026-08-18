using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using Newtonsoft.Json.Linq;
using ManagerServer.Model;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json.Serialization;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.HttpHandlers.Businesses.Business.Reports;

namespace ManagerServer.HttpHandlers.Api2
{
    [ProtoContract]
    public sealed class Api2 : HttpHandler
    {
        public override async Task ProcessRequest()
        {
            if (Request.Method == "OPTIONS") return;

            Response.ContentType = "application/json";

            var path = Request.Path.Value.Substring(('/' + this.GetType().Name).Length);

            if (path == string.Empty)
            {
                var url = Request.Scheme + "://" + Request.Host + Request.Path.ToString();
                if (Request.Headers.TryGetValue("X-Forwarded-Proto", out Microsoft.Extensions.Primitives.StringValues value))
                {
                    if (value == "https")
                    {
                        url = "https://" + Request.Host + Request.Path.ToString();
                    }
                }

                Write(CreateOpenApiSpec(url));
                return;
            }
            else
            {
                var path2 = path.Substring(1).Split('/').First();
                if (!Assembly.ContainsHttpHandler(path2))
                {
                    Response.StatusCode = 404;
                    return;
                }

                var handlerType = Assembly.GetHttpHandlerType(path2);

                MapApiExtension.UserContext userContext = null;
                try { userContext = MapApiExtension.GetUserContext(HttpContext.Request); }
                catch (MapApiExtension.HttpRequestException) { /* unauthenticated — fall through to X-API-KEY check below */ }

                if (userContext != null && userContext.UserPermissions != null)
                {
                    if (userContext != null)
                    {
                        var business = userContext.Business;

                        var userPermissions = userContext.UserPermissions;

                        var dict = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

                        try
                        {
                            if (handlerType.IsSubclassOf(typeof(NakedObjectsWithJsonOutput)))
                            {
                                if (userPermissions.CanView(handlerType.Namespace))
                                {
                                    var httpHandler = JsonConvert.DeserializeObject(json, handlerType) as NakedObjectsWithJsonOutput;
                                    httpHandler.Business = business;
                                    httpHandler.ContentOnly = true;
                                    httpHandler.JsonOutput = true;
                                    httpHandler.HttpContext = HttpContext;
                                    httpHandler.StringBuilder = new System.Text.StringBuilder(32000);

                                    await httpHandler.Get();
                                    return;
                                }
                                else
                                {
                                    Response.StatusCode = 401;
                                    var error = new JObject();
                                    error["error"] = "User does not have user pemissions to view this resource";
                                    Write(error.ToString());
                                    return;
                                }
                            }
                            if (handlerType.IsSubclassOf(typeof(VueForm)))
                            {
                                var objectType = handlerType.BaseType.GetGenericArguments()[0];
                                if (Request.Method == "GET")
                                {
                                    if (userPermissions.CanView(handlerType.Namespace))
                                    {
                                        var key = path.Substring(1).Split('/').Last().ToGuid();
                                        var o = ApplicationData.Businesses.Get(business).SingleOrDefault(key);
                                        if (o != null && o.GetType() == objectType)
                                        {
                                            Response.StatusCode = 200;
                                            var json2 = Serialize(o);
                                            await Response.WriteAsync(json2);
                                            return;
                                        }
                                        else
                                        {
                                            Response.StatusCode = 404;
                                            var error = new JObject();
                                            error["error"] = "Not found";
                                            Write(error.ToString());
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Response.StatusCode = 401;
                                        var error = new JObject();
                                        error["error"] = "User does not have user pemissions to view this resource";
                                        Write(error.ToString());
                                        return;
                                    }
                                }

                                if (Request.Method == "DELETE")
                                {
                                    if (userPermissions.CanDelete(handlerType.Namespace))
                                    {
                                        var key = path.Substring(1).Split('/').Last().ToGuid();
                                        ApplicationData.Businesses.Process(business, key, "API");
                                        Response.StatusCode = 204;
                                        return;
                                    }
                                    else
                                    {
                                        Response.StatusCode = 401;
                                        var error = new JObject();
                                        error["error"] = "User does not have user pemissions to delete this resource";
                                        Write(error.ToString());
                                        return;
                                    }
                                }

                                if (Request.Method == "PUT")
                                {
                                    if (userPermissions.CanUpdate(handlerType.Namespace))
                                    {
                                        var key = path.Substring(1).Split('/').Last().ToGuid();
                                        var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                                        var o = JsonConvert.DeserializeObject(rawRequestBody, objectType) as ManagerServer.Model.Object;
                                        o.Key = key;
                                        ApplicationData.Businesses.Process(business, o, "API");
                                        Response.StatusCode = 200;
                                        return;
                                    }
                                    else
                                    {
                                        Response.StatusCode = 401;
                                        var error = new JObject();
                                        error["error"] = "User does not have user pemissions to update this resource";
                                        Write(error.ToString());
                                        return;
                                    }
                                }

                                if (Request.Method == "POST")
                                {
                                    if (userPermissions.CanCreate(handlerType.Namespace))
                                    {
                                        var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                                        var o = JsonConvert.DeserializeObject(rawRequestBody, objectType) as ManagerServer.Model.Object;
                                        ApplicationData.Businesses.Process(business, o, "API");
                                        Response.StatusCode = 201;
                                        var json2 = Serialize(o);
                                        await Response.WriteAsync(json2);
                                        return;
                                    }
                                    else
                                    {
                                        Response.StatusCode = 401;
                                        var error = new JObject();
                                        error["error"] = "User does not have user pemissions to create this resource";
                                        Write(error.ToString());
                                        return;
                                    }
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            Response.StatusCode = 500;
                            var error = new JObject();
                            error["error"] = ex.ToString();
                            Write(error.ToString());
                            return;
                        }
                    }
                }

                if (Request.Headers.TryGetValue("X-API-KEY", out Microsoft.Extensions.Primitives.StringValues value))
                {
                    try
                    {
                        Convert.FromBase64String(value);
                    }
                    catch (FormatException)
                    {
                        Response.StatusCode = 401;
                        var formatException = new JObject() ;
                        formatException["error"] = "X-API-KEY is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.";
                        Write(formatException.ToString());
                        return;
                    }

                    var buffer = Convert.FromBase64String(value);
                    using (var ms = new System.IO.MemoryStream(buffer))
                    {
                        Tuple<string, Guid, Guid> tuple;
                        try
                        {
                            tuple = ProtoBuf.Serializer.Deserialize<Tuple<string, Guid, Guid>>(ms);
                        }
                        catch (ProtoException)
                        {
                            Response.StatusCode = 401;
                            var formatException = new JObject();
                            formatException["error"] = "X-API-KEY is not a valid string.";
                            Write(formatException.ToString());
                            return;
                        }

                        var database = ApplicationData.Businesses.Get(tuple.Item1);
                        if (database != null)
                        {
                            var accessToken = database.SingleOrDefault<AccessToken>(tuple.Item2);
                            if (accessToken != null)
                            {
                                if (BCrypt.Net.BCrypt.Verify(tuple.Item3.ToString(), accessToken.Password))
                                {
                                    var dict = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
                                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

                                    try
                                    {
                                        if (handlerType.IsSubclassOf(typeof(NakedObjectsWithJsonOutput)))
                                        {
                                            var httpHandler = JsonConvert.DeserializeObject(json, handlerType) as NakedObjectsWithJsonOutput;
                                            httpHandler.Business = tuple.Item1;
                                            httpHandler.ContentOnly = true;
                                            httpHandler.JsonOutput = true;
                                            httpHandler.HttpContext = HttpContext;
                                            httpHandler.StringBuilder = new System.Text.StringBuilder(32000);

                                            await httpHandler.Get();
                                            return;
                                        }
                                        if (handlerType.IsSubclassOf(typeof(VueForm)))
                                        {
                                            var objectType = handlerType.BaseType.GetGenericArguments()[0];
                                            if (Request.Method == "GET")
                                            {
                                                var key = path.Substring(1).Split('/').Last().ToGuid();
                                                var o = ApplicationData.Businesses.Get(tuple.Item1).SingleOrDefault(key);
                                                if (o != null && o.GetType() == objectType)
                                                {
                                                    Response.StatusCode = 200;
                                                    var json2 = Serialize(o);
                                                    await Response.WriteAsync(json2);
                                                    return;
                                                }
                                                else
                                                {
                                                    Response.StatusCode = 404;
                                                    var error = new JObject();
                                                    error["error"] = "Not found";
                                                    Write(error.ToString());
                                                    return;
                                                }
                                            }

                                            if (Request.Method == "DELETE")
                                            {
                                                var key = path.Substring(1).Split('/').Last().ToGuid();
                                                ApplicationData.Businesses.Process(tuple.Item1, key, "API");
                                                Response.StatusCode = 204;
                                                return;
                                            }

                                            if (Request.Method == "PUT")
                                            {
                                                var key = path.Substring(1).Split('/').Last().ToGuid();
                                                var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                                                var o = JsonConvert.DeserializeObject(rawRequestBody, objectType) as ManagerServer.Model.Object;
                                                o.Key = key;
                                                ApplicationData.Businesses.Process(tuple.Item1, o, "API");
                                                Response.StatusCode = 200;
                                                return;
                                            }

                                            if (Request.Method == "POST")
                                            {
                                                var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
                                                var o = JsonConvert.DeserializeObject(rawRequestBody, objectType) as ManagerServer.Model.Object;
                                                ApplicationData.Businesses.Process(tuple.Item1, o, "API");
                                                Response.StatusCode = 201;
                                                var json2 = Serialize(o);
                                                await Response.WriteAsync(json2);
                                                return;
                                            }
                                        }                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        Response.StatusCode = 500;
                                        var error = new JObject();
                                        error["error"] = ex.ToString();
                                        Write(error.ToString());
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Response.StatusCode = 401;
            var unauthorizedError = new JObject();
            unauthorizedError["error"] = "Unauthorized";
            Write(unauthorizedError.ToString());
            return;
        }

        private string Serialize(object o)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new OnlyFieldsContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(o, Formatting.Indented, settings);
        }

        public class OnlyFieldsContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var members = type.GetFieldsAndProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return members.Select(m => base.CreateProperty(m, memberSerialization)).ToList();
            }
        }

        public string CreateOpenApiSpec(string url)
        {
            var openApiDocument = new JObject();
            openApiDocument["openapi"] = "3.0.0";
            openApiDocument["info"] = new JObject();
            openApiDocument["info"]["title"] = "Manager API";
            openApiDocument["info"]["version"] = this.GetType().Assembly.GetName().Version.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                openApiDocument["servers"] = new JArray(new JObject());
                openApiDocument["servers"][0]["url"] = url;
            }
            openApiDocument["paths"] = new JObject();

            var tableTypes = typeof(Api2).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(NakedObjectsWithJsonOutput))).ToArray();

            foreach (var tableType in tableTypes.OrderBy(x => Strings.GetPropertyValue(x.Name)))
            {
                var parameters = new List<JObject>();

                var table = (NakedObjectsWithJsonOutput)Activator.CreateInstance(tableType);

                var columns = table.GetColumns().Where(x => x.CanConvertToJson && !string.IsNullOrWhiteSpace(x.Name)).ToArray();

                var fields = tableType.GetFieldsAndProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var jsonProperty = field.GetCustomAttribute<JsonPropertyAttribute>();
                    if (jsonProperty == null) continue;

                    var parameter = new JObject();
                    parameter["name"] = jsonProperty.PropertyName;
                    parameter["required"] = false;
                    parameter["in"] = "query";
                    parameter["schema"] = new JObject();
                    if (field.GetMemberType() == typeof(int) || field.GetMemberType() == typeof(int?))
                    {
                        parameter["schema"]["type"] = "integer";
                    }
                    else if (field.GetMemberType() == typeof(bool))
                    {
                        parameter["schema"]["type"] = "boolean";
                    }
                    else if (field.Name == nameof(NakedObjectsWithJsonOutput.Fields))
                    {
                        parameter["schema"] = new JObject();
                        parameter["schema"]["type"] = "array";
                        parameter["schema"]["items"] = new JObject();
                        parameter["schema"]["items"]["type"] = "string";
                        parameter["schema"]["items"]["enum"] = new JArray(columns.Select(x => x.Name).ToArray());
                    }
                    else if (field.Name == nameof(NakedObjectsWithSorting.SortByString))
                    {
                        parameter["schema"]["type"] = "string";
                        parameter["schema"]["enum"] = new JArray(columns.Select(x => x.Name).ToArray());
                    }
                    else
                    {
                        parameter["schema"]["type"] = "string";
                    }
                    parameters.Add(parameter);
                }

                var path = $"{HttpHandler.ConvertPascalToKebabCase(tableType)}";
                openApiDocument["paths"][path] = new JObject();
                openApiDocument["paths"][path]["get"] = new JObject();
                openApiDocument["paths"][path]["get"]["operationId"] = "Get"+tableType.Name;
                openApiDocument["paths"][path]["get"]["summary"] = Strings.GetPropertyValue(tableType.Name);
                openApiDocument["paths"][path]["get"]["description"] = "Retrieve list of " + Strings.GetPropertyValue(tableType.Name).ToLowerInvariant()+$". For more information, see https://www.manager.io/guides?{path.Substring(1)}";
                openApiDocument["paths"][path]["get"]["parameters"] = new JArray(parameters);
                openApiDocument["paths"][path]["get"]["responses"] = new JObject();
                openApiDocument["paths"][path]["get"]["responses"]["200"] = new JObject();
                openApiDocument["paths"][path]["get"]["responses"]["200"]["description"] = "Successful response";
                openApiDocument["paths"][path]["get"]["responses"]["200"]["content"] = new JObject();
                openApiDocument["paths"][path]["get"]["responses"]["200"]["content"]["application/json"] = new JObject();
            }

            var formTypes = typeof(Api2).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(VueForm))).ToArray();
            foreach (var formType in formTypes)
            {
                var path = $"{HttpHandler.ConvertPascalToKebabCase(formType)}";
                openApiDocument["paths"][path] = new JObject();
                openApiDocument["paths"][path]["post"] = new JObject();
                openApiDocument["paths"][path]["post"]["operationId"] = "Post" + formType.Name;
                openApiDocument["paths"][path]["post"]["summary"] = Strings.GetPropertyValue(formType.Name);
                openApiDocument["paths"][path]["post"]["description"] = "Create a new resource";
                openApiDocument["paths"][path]["post"]["requestBody"] = new JObject();
                openApiDocument["paths"][path]["post"]["requestBody"]["required"] = true;
                openApiDocument["paths"][path]["post"]["requestBody"]["content"] = new JObject();
                openApiDocument["paths"][path]["post"]["requestBody"]["content"]["application/json"] = new JObject();
                openApiDocument["paths"][path]["post"]["requestBody"]["content"]["application/json"]["schema"] = new JObject();
                openApiDocument["paths"][path]["post"]["requestBody"]["content"]["application/json"]["schema"]["type"] = "object";
                openApiDocument["paths"][path]["post"]["responses"] = new JObject();
                openApiDocument["paths"][path]["post"]["responses"]["201"] = new JObject();
                openApiDocument["paths"][path]["post"]["responses"]["201"]["description"] = "Resource created";

                var parameters = new JArray(new JObject());
                parameters[0]["name"] = "key";
                parameters[0]["in"] = "path";
                parameters[0]["required"] = true;
                parameters[0]["schema"] = new JObject();
                parameters[0]["schema"]["type"] = "string";
                parameters[0]["schema"]["format"] = "uuid";

                var path2 = $"{path}/{{key}}";
                
                openApiDocument["paths"][path2] = new JObject();

                openApiDocument["paths"][path2]["get"] = new JObject();
                openApiDocument["paths"][path2]["get"]["operationId"] = "Get" + formType.Name;
                openApiDocument["paths"][path2]["get"]["summary"] = "Get a resource by Key";
                openApiDocument["paths"][path2]["get"]["parameters"] = parameters;
                openApiDocument["paths"][path2]["get"]["responses"] = new JObject();
                openApiDocument["paths"][path2]["get"]["responses"]["200"] = new JObject();
                openApiDocument["paths"][path2]["get"]["responses"]["200"]["description"] = "Success";

                openApiDocument["paths"][path2]["delete"] = new JObject();
                openApiDocument["paths"][path2]["delete"]["operationId"] = "Delete" + formType.Name;
                openApiDocument["paths"][path2]["delete"]["summary"] = "Delete a resource by Key";
                openApiDocument["paths"][path2]["delete"]["parameters"] = parameters;
                openApiDocument["paths"][path2]["delete"]["responses"] = new JObject();
                openApiDocument["paths"][path2]["delete"]["responses"]["204"] = new JObject();
                openApiDocument["paths"][path2]["delete"]["responses"]["204"]["description"] = "No Content";

                openApiDocument["paths"][path2]["put"] = new JObject();
                openApiDocument["paths"][path2]["put"]["operationId"] = "Put" + formType.Name;
                openApiDocument["paths"][path2]["put"]["summary"] = "Put a resource by Key";
                openApiDocument["paths"][path2]["put"]["parameters"] = parameters;
                openApiDocument["paths"][path2]["put"]["requestBody"] = new JObject();
                openApiDocument["paths"][path2]["put"]["requestBody"]["required"] = true;
                openApiDocument["paths"][path2]["put"]["requestBody"]["content"] = new JObject();
                openApiDocument["paths"][path2]["put"]["requestBody"]["content"]["application/json"] = new JObject();
                openApiDocument["paths"][path2]["put"]["requestBody"]["content"]["application/json"]["schema"] = new JObject();
                openApiDocument["paths"][path2]["put"]["requestBody"]["content"]["application/json"]["schema"]["type"] = "object";
                openApiDocument["paths"][path2]["put"]["responses"] = new JObject();
                openApiDocument["paths"][path2]["put"]["responses"]["200"] = new JObject();
                openApiDocument["paths"][path2]["put"]["responses"]["200"]["description"] = "Success";
            }            

            openApiDocument["components"] = new JObject();
            openApiDocument["components"]["schemas"] = new JObject();
            openApiDocument["components"]["securitySchemes"] = new JObject();
            openApiDocument["components"]["securitySchemes"]["ApiKeyAuth"] = new JObject();
            openApiDocument["components"]["securitySchemes"]["ApiKeyAuth"]["type"] = "apiKey";
            openApiDocument["components"]["securitySchemes"]["ApiKeyAuth"]["in"] = "header";
            openApiDocument["components"]["securitySchemes"]["ApiKeyAuth"]["name"] = "X-API-KEY";

            openApiDocument["security"] = new JArray(new JObject());
            openApiDocument["security"][0]["ApiKeyAuth"] = new JArray();

            openApiDocument["x-readme"] = new JObject();
            openApiDocument["x-readme"]["proxy-enabled"] = false;

            return openApiDocument.ToString();
        }
    }
}

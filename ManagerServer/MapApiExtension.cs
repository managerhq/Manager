using HttpFramework;
using ManagerServer;
using ManagerServer.Authentication;
using ManagerServer.HttpHandlers.Api2;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManagerServer
{
    public static class MapApiExtension
    {
        internal static UserContext GetUserContext(HttpRequest httpRequest)
        {
            var user = httpRequest.HttpContext.GetManagerUser();
            if (user == null)
            {
                throw new HttpRequestException() { Result = Results.Unauthorized() };
            }

            var business = Uri.UnescapeDataString(httpRequest.Headers["Manager-Business"].ToString());
            if (string.IsNullOrWhiteSpace(business))
            {
                return new UserContext() { Username = user.Username };
            }

            var userPermissions = new ManagerServer.Model.UserPermissions();
            if (Edition.IsDesktop) userPermissions = new ManagerServer.Model.UserPermissions() { AccessType = ManagerServer.Model.Enums.UserPermissionsAccessType.FullAccess };
            if (user.Type == UserType.Administrator) userPermissions = new ManagerServer.Model.UserPermissions() { AccessType = ManagerServer.Model.Enums.UserPermissionsAccessType.FullAccess };

            if (user.Businesses != null && user.Businesses.Contains(business))
            {
                userPermissions = ApplicationData.Instance.Businesses.Get(business)?.OfType<ManagerServer.Model.UserPermissions>().OrderBy(x => x.Key).FirstOrDefault(x => x.Username == user.Username);
                if (userPermissions == null) userPermissions = new ManagerServer.Model.UserPermissions();
            }

            return new UserContext() { Business = business, Username = user.Username, UserPermissions = userPermissions };
        }

        private static JsonElement MergePatch(JsonElement original, JsonElement patch)
        {
            if (patch.ValueKind != JsonValueKind.Object)
                return patch;

            var result = new Dictionary<string, JsonElement>();

            foreach (var prop in original.EnumerateObject())
                result[prop.Name] = prop.Value;

            foreach (var prop in patch.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Null)
                    result.Remove(prop.Name);
                else
                    result[prop.Name] = result.TryGetValue(prop.Name, out var existing) && existing.ValueKind == JsonValueKind.Object && prop.Value.ValueKind == JsonValueKind.Object
                        ? MergePatch(existing, prop.Value)
                        : prop.Value;
            }

            var mergedJson = System.Text.Json.JsonSerializer.Serialize(result);
            return JsonDocument.Parse(mergedJson).RootElement;
        }

        public sealed class UserContext
        {
            public string Username;
            public UserPermissions UserPermissions;
            public string Business;
        }

        public sealed class HttpRequestException : Exception
        {
            public IResult Result { get; set; }
        }

        public static void MapApi(this WebApplication app)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                //IgnoreReadOnlyProperties = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = new ProtoAwareResolver()
            };

            var group = app.MapGroup("").WithGroupName("obsolete");

            var formTypes = typeof(Api2).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(VueForm))).ToArray();
            foreach (var formType in formTypes)
            {
                var path = $"{HttpHandler.ConvertPascalToKebabCase(formType)}";
                group.MapGet($"/api3{path}", (HttpContext context) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);
                        if (!userContext.UserPermissions.CanView(formType.Namespace)) return Results.Unauthorized();
                        var objectType = formType.BaseType.GetGenericArguments()[0];
                        var key = ManagerServer.Model.Object.GetGuidByType(objectType);

                        return Results.Json(ApplicationData.Instance.Businesses.Get(userContext.Business).Single(key), options);
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapGet($"/api3{path}/{{key:Guid}}", (HttpContext context, Guid key) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        var o = ApplicationData.Instance.Businesses.Get(userContext.Business).SingleOrDefault(key);

                        if (o == null) return Results.NotFound();

                        if (!userContext.UserPermissions.CanView(formType.Namespace))
                        {
                            return Results.NoContent();
                        }

                        return Results.Json(o, options);
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapPut($"/api3{path}", async (HttpContext context) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        if (!userContext.UserPermissions.CanUpdate(formType.Namespace)) return Results.Unauthorized();

                        var objectType = formType.BaseType.GetGenericArguments()[0];
                        var key = ManagerServer.Model.Object.GetGuidByType(objectType);

                        using var reader = new StreamReader(context.Request.Body);
                        var json = await reader.ReadToEndAsync();

                        var o = System.Text.Json.JsonSerializer.Deserialize(json, objectType, options) as ManagerServer.Model.Object;
                        o.Key = key;

                        ApplicationData.Instance.Businesses.Process(userContext.Business, o, userContext.Username);

                        return Results.Accepted();
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapPut($"/api3{path}/{{key:Guid}}", async (HttpContext context, Guid key) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        if (!userContext.UserPermissions.CanUpdate(formType.Namespace)) return Results.Unauthorized();

                        var objectType = formType.BaseType.GetGenericArguments()[0];

                        using var reader = new StreamReader(context.Request.Body);
                        var json = await reader.ReadToEndAsync();

                        var o = System.Text.Json.JsonSerializer.Deserialize(json, objectType, options) as ManagerServer.Model.Object;
                        o.Key = key;

                        ApplicationData.Instance.Businesses.Process(userContext.Business, o, userContext.Username);

                        return Results.Accepted();
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapDelete($"/api3{path}/{{key:Guid}}", (HttpContext context, Guid key) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        if (!userContext.UserPermissions.CanUpdate(formType.Namespace)) return Results.Unauthorized();
                       
                        ApplicationData.Instance.Businesses.Process(userContext.Business, key, userContext.Username);

                        return Results.Accepted();
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapPatch($"/api3{path}", async (HttpContext context) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        if (!userContext.UserPermissions.CanUpdate(formType.Namespace))
                            return Results.Unauthorized();

                        var objectType = formType.BaseType.GetGenericArguments()[0];
                        var key = ManagerServer.Model.Object.GetGuidByType(objectType);
                        var existing = ApplicationData.Instance.Businesses.Get(userContext.Business).Single(key);

                        if (existing == null) return Results.NotFound();

                        using var reader = new StreamReader(context.Request.Body);
                        var json = await reader.ReadToEndAsync();

                        // Parse incoming patch as JsonDocument and merge manually
                        using var doc = JsonDocument.Parse(json);
                        var existingJson = System.Text.Json.JsonSerializer.SerializeToDocument(existing, objectType, options);

                        var merged = MergePatch(existingJson.RootElement, doc.RootElement); // You'll define this
                        var patched = merged.Deserialize(objectType, options) as ManagerServer.Model.Object;
                        patched.Key = key;

                        ApplicationData.Instance.Businesses.Process(userContext.Business, patched, userContext.Username);

                        return Results.Accepted();
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapPatch($"/api3{path}/{{key:Guid}}", async (HttpContext context, Guid key) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        if (!userContext.UserPermissions.CanUpdate(formType.Namespace))
                            return Results.Unauthorized();

                        var objectType = formType.BaseType.GetGenericArguments()[0];
                        var existing = ApplicationData.Instance.Businesses.Get(userContext.Business).SingleOrDefault(key);

                        if (existing == null) return Results.NotFound();

                        using var reader = new StreamReader(context.Request.Body);
                        var json = await reader.ReadToEndAsync();

                        // Parse incoming patch as JsonDocument and merge manually
                        using var doc = JsonDocument.Parse(json);
                        var existingJson = System.Text.Json.JsonSerializer.SerializeToDocument(existing, objectType, options);

                        var merged = MergePatch(existingJson.RootElement, doc.RootElement); // You'll define this
                        var patched = merged.Deserialize(objectType, options) as ManagerServer.Model.Object;
                        patched.Key = key;

                        ApplicationData.Instance.Businesses.Process(userContext.Business, patched, userContext.Username);

                        return Results.Accepted();
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });

                group.MapPost($"/api3{path}", async (HttpContext context) =>
                {
                    try
                    {
                        var userContext = GetUserContext(context.Request);

                        if (!userContext.UserPermissions.CanCreate(formType.Namespace)) return Results.Unauthorized();

                        var objectType = formType.BaseType.GetGenericArguments()[0];

                        using var reader = new StreamReader(context.Request.Body);
                        var json = await reader.ReadToEndAsync();

                        var o = Newtonsoft.Json.JsonConvert.DeserializeObject(json, objectType) as ManagerServer.Model.Object;
                        o.Key = Guid.CreateVersion7();

                        ApplicationData.Instance.Businesses.Process(userContext.Business, o, userContext.Username);

                        return Results.Ok(o.Key);
                    }
                    catch (HttpRequestException ex)
                    {
                        return ex.Result;
                    }
                    catch (Exception ex)
                    {
                        return Results.InternalServerError(ex.Message);
                    }
                });                
            }            

            group.MapPatch($"/api3/objects/{{key:Guid}}", async (HttpContext context, Guid key) =>
            {
                try
                {
                    var userContext = GetUserContext(context.Request);

                    if (userContext.UserPermissions.AccessType != ManagerServer.Model.Enums.UserPermissionsAccessType.FullAccess)
                    {
                        return Results.Unauthorized();
                    }

                    /*
                    if (!userContext.UserPermissions.CanUpdate(formType.Namespace))
                        return Results.Unauthorized();
                    */

                    //var objectType = formType.BaseType.GetGenericArguments()[0];

                    var existing = ApplicationData.Instance.Businesses.Get(userContext.Business).Single(key) ?? ApplicationData.Instance.Businesses.Get(userContext.Business).SingleOrDefault(key);

                    if (existing == null) return Results.NotFound();

                    using var reader = new StreamReader(context.Request.Body);
                    var json = await reader.ReadToEndAsync();

                    // Parse incoming patch as JsonDocument and merge manually
                    using var doc = JsonDocument.Parse(json);
                    var existingJson = System.Text.Json.JsonSerializer.SerializeToDocument(existing, existing.GetType(), options);

                    var merged = MergePatch(existingJson.RootElement, doc.RootElement); // You'll define this
                    var patched = merged.Deserialize(existing.GetType(), options) as ManagerServer.Model.Object;
                    patched.Key = key;

                    ApplicationData.Instance.Businesses.Process(userContext.Business, patched, userContext.Username);

                    return Results.Accepted();
                }
                catch (HttpRequestException ex)
                {
                    return ex.Result;
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

            group.MapGet($"/api3/chart-of-accounts", (HttpContext context) =>
            {
                try
                {
                    var userContext = GetUserContext(context.Request);
                    if (!userContext.UserPermissions.CanView(typeof(HttpHandlers.Businesses.Business.Settings.ChartOfAccounts.ChartOfAccounts).Namespace)) return Results.Unauthorized();

                    var o = new HttpHandlers.Businesses.Business.Settings.ChartOfAccounts.ChartOfAccounts() { Business = userContext.Business }.GetModel();

                    return Results.Json(o, options);
                }
                catch (HttpRequestException ex)
                {
                    return ex.Result;
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

            group.MapPost($"/api3/blobs", async (HttpContext context) =>
            {
                try
                {
                    var userContext = GetUserContext(context.Request);
                    using var reader = new StreamReader(context.Request.Body);
                    var json = await reader.ReadToEndAsync();
                    var blob = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerServer.ApplicationData.Blob>(json);
                    var key = ApplicationData.Instance.Businesses.InsertBlob(userContext.Business, blob);
                    return Results.Ok(key);
                }
                catch (HttpRequestException ex)
                {
                    return ex.Result;
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            }).AddEndpointFilter(async (context, next) =>
            {
                var maxBytes = 1024 * 1024; // 1 MB
                context.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>()!.MaxRequestBodySize = maxBytes;
                return await next(context);
            });
        }
    }
}

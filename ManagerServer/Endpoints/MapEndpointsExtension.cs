using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace ManagerServer.Endpoints
{
    public static class MapEndpointsExtension
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly (string Prefix, string Method)[] Prefixes =
        [
            ("Delete", HttpMethods.Delete),
            ("Patch",  HttpMethods.Patch),
            ("Post",   HttpMethods.Post),
            ("Put",    HttpMethods.Put),
            ("Get",    HttpMethods.Get),
            ("Query",  "QUERY"),
        ];

        private static readonly MethodInfo HandleFromQueryMethod = typeof(MapEndpointsExtension)
            .GetMethod(nameof(HandleFromQuery), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo HandleFromBodyMethod = typeof(MapEndpointsExtension)
            .GetMethod(nameof(HandleFromBody), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly string AssemblyVersion =
            typeof(MapEndpointsExtension).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? typeof(MapEndpointsExtension).Assembly.GetName().Version?.ToString()
            ?? "0.0.0";

        public static IEnumerable<string> GetOpenApiDocumentNames()
        {
            foreach (var e in Discover())
                yield return Endpoint.ToKebabCase(e.EndpointType);
        }

        public static IServiceCollection AddEndpointsOpenApi(this IServiceCollection services)
        {
            foreach (var e in Discover())
            {
                var documentName = Endpoint.ToKebabCase(e.EndpointType);
                services.AddOpenApi(documentName, options =>
                {
                    options.ShouldInclude = description => description.GroupName == documentName;
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        document.Servers.Clear();
                        document.Info.Version = AssemblyVersion;
                        return System.Threading.Tasks.Task.CompletedTask;
                    });
                });
            }
            return services;
        }

        public static void MapEndpoints(this WebApplication app)
        {
            foreach (var e in Discover())
            {
                var hasBody = e.HttpMethod == HttpMethods.Post || e.HttpMethod == HttpMethods.Put || e.HttpMethod == HttpMethods.Patch || e.HttpMethod == "QUERY";
                var openMethod = hasBody ? HandleFromBodyMethod : HandleFromQueryMethod;
                var closedMethod = openMethod.MakeGenericMethod(e.EndpointType, e.ResultType);
                var delegateType = hasBody
                    ? typeof(Func<,,>).MakeGenericType(e.EndpointType, typeof(HttpContext), typeof(IResult))
                    : typeof(Func<,>).MakeGenericType(e.EndpointType, typeof(IResult));
                var del = closedMethod.CreateDelegate(delegateType);

                var produces = e.EndpointType.GetCustomAttribute<ProducesContentAttribute>();
                var successContentType = produces?.ContentType ?? "application/json";
                var successBodyType = produces?.BodyType ?? e.ResultType;

                var routeBuilder = app.MapMethods(e.Path, [e.HttpMethod], del)
                    .WithGroupName(Endpoint.ToKebabCase(e.EndpointType))
                    .WithTags()
                    .Produces(StatusCodes.Status200OK, successBodyType, successContentType)
                    .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
                    .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized, "application/json")
                    .Produces<ErrorResponse>(StatusCodes.Status403Forbidden, "application/json");

                var obsolete = e.EndpointType.GetCustomAttribute<System.ObsoleteAttribute>();
                if (obsolete != null)
                {
                    routeBuilder.WithMetadata(obsolete);
                }
            }

            app.MapFallback("/api4/{**path}", NotFoundResult).ExcludeFromDescription();
        }

        private static IResult NotFoundResult(HttpContext context) => new NotFoundJsonResult(context.Request.Method, context.Request.Path.Value);

        private sealed class NotFoundJsonResult : IResult
        {
            private readonly string _method;
            private readonly string _path;

            public NotFoundJsonResult(string method, string path)
            {
                _method = method;
                _path = path;
            }

            public async System.Threading.Tasks.Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                var body = new ErrorResponse
                {
                    Error = "not_found",
                    Message = $"No endpoint matches {_method} {_path}."
                };
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, body, JsonOptions);
            }
        }

        private readonly record struct DiscoveredEndpoint(Type EndpointType, Type ResultType, string HttpMethod, string Path);

        private static IEnumerable<DiscoveredEndpoint> Discover()
        {
            foreach (var endpointType in typeof(Program).Assembly.GetTypes())
            {
                if (!endpointType.IsClass || endpointType.IsAbstract) continue;
                if (endpointType.GetConstructor(Type.EmptyTypes) == null) continue;

                var resultType = GetEndpointResultType(endpointType);
                if (resultType == null) continue;

                var (prefix, httpMethod) = MatchPrefix(endpointType.Name);
                if (prefix == null) continue;

                var path = Endpoint.ToUrl(endpointType);

                yield return new DiscoveredEndpoint(endpointType, resultType, httpMethod, path);
            }
        }

        private static IResult HandleFromQuery<TEndpoint, TResult>([AsParameters] TEndpoint endpoint) where TEndpoint : Endpoint<TResult>
        {
            try
            {
                var result = endpoint.Handle();
                return result is IResult r ? r : Results.Json(result, JsonOptions);
            }
            catch (UnauthenticatedException)
            {
                return UnauthorizedResult();
            }
            catch (BadRequestException ex)
            {
                return BadRequestResult(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return ForbiddenResult(ex.Message);
            }
        }

        private static IResult HandleFromBody<TEndpoint, TResult>(TEndpoint endpoint, HttpContext httpContext) where TEndpoint : Endpoint<TResult>
        {
            endpoint.Context = httpContext;
            try
            {
                var result = endpoint.Handle();
                return result is IResult r ? r : Results.Json(result, JsonOptions);
            }
            catch (UnauthenticatedException)
            {
                return UnauthorizedResult();
            }
            catch (BadRequestException ex)
            {
                return BadRequestResult(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return ForbiddenResult(ex.Message);
            }
        }

        private static IResult UnauthorizedResult() => new UnauthorizedJsonResult();
        private static IResult BadRequestResult(string message) => new BadRequestJsonResult(message);
        private static IResult ForbiddenResult(string message) => new ForbiddenJsonResult(message);

        private sealed class UnauthorizedJsonResult : IResult
        {
            private static readonly ErrorResponse Body = new ErrorResponse
            {
                Error = "authentication_required",
                Message = "Authentication is required to access this endpoint."
            };

            public async System.Threading.Tasks.Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Manager\"";
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, Body, JsonOptions);
            }
        }

        private sealed class BadRequestJsonResult : IResult
        {
            private readonly string _message;

            public BadRequestJsonResult(string message)
            {
                _message = message;
            }

            public async System.Threading.Tasks.Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                var body = new ErrorResponse { Error = "bad_request", Message = _message };
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, body, JsonOptions);
            }
        }

        private sealed class ForbiddenJsonResult : IResult
        {
            private readonly string _message;

            public ForbiddenJsonResult(string message)
            {
                _message = message;
            }

            public async System.Threading.Tasks.Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                var body = new ErrorResponse { Error = "forbidden", Message = _message };
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, body, JsonOptions);
            }
        }

        private static Type GetEndpointResultType(Type type)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Endpoint<>))
                {
                    return t.GetGenericArguments()[0];
                }
            }
            return null;
        }

        private static (string Prefix, string HttpMethod) MatchPrefix(string typeName)
        {
            foreach (var (prefix, method) in Prefixes)
            {
                if (typeName.Length > prefix.Length
                    && typeName.StartsWith(prefix, StringComparison.Ordinal)
                    && char.IsUpper(typeName[prefix.Length]))
                {
                    return (prefix, method);
                }
            }
            return (null, null);
        }
    }
}

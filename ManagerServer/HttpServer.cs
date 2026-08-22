using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ManagerServer.Extensions;

#if !DEBUG
using Microsoft.AspNetCore.Diagnostics;
#endif

namespace ManagerServer
{
    public static class HttpServer
    {
        private static async Task<WebApplication> Build(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var openApiPublishUrl = builder.Configuration["openapi-publish-url"];
            var openApiOutputDir = builder.Configuration["openapi-output-dir"];
            if (!string.IsNullOrEmpty(openApiPublishUrl) || !string.IsNullOrEmpty(openApiOutputDir))
            {
                builder.WebHost.UseUrls("http://127.0.0.1:0");
            }

            builder.Services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/x-manager"]);
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(x => x.Level = System.IO.Compression.CompressionLevel.Optimal);
            builder.Services.Configure<BrotliCompressionProviderOptions>(x => x.Level = System.IO.Compression.CompressionLevel.Optimal);

            builder.Services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = int.MaxValue;
                x.KeyLengthLimit = int.MaxValue;
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartHeadersCountLimit = int.MaxValue;
                x.MultipartBoundaryLengthLimit = int.MaxValue;
            });

            builder.Services.AddLogging(x => x.AddSimpleConsole(c => c.SingleLine = true));

            var sentryDsn = builder.Configuration["sentry"];
            var servedBy = builder.Configuration["served-by"];
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                builder.WebHost.UseSentry(o =>
                {
                    o.Dsn = sentryDsn;
                    o.Release = typeof(Program).Assembly.GetName().Version.ToString();
                    o.AddExceptionFilterForType<BadHttpRequestException>();
                    o.AddExceptionFilterForType<OutOfMemoryException>();
                    o.TracesSampleRate = 0.01;
                    if (!string.IsNullOrEmpty(servedBy))
                    {
                        o.DefaultTags["served-by"] = servedBy;
                    }
                });
            }

            var pdf = builder.Configuration["pdf"];
            if (!string.IsNullOrEmpty(pdf))
            {
                var token = pdf.Split('@').First();
                var endpoint = pdf.Split('@').Last();
                builder.Services.AddSingleton(new Services.Pdf(token, endpoint));
            }

            var smtp = builder.Configuration["smtp"];
            if (!string.IsNullOrEmpty(smtp))
            {
                builder.Services.AddSingleton(new Services.SmtpSettings(new Uri(smtp)));
            }

            builder.Services.AddSingleton(ApplicationData.Instance);
            builder.Services.AddHostedService<Services.StorageHousekeepingService>();

            builder.Services.AddSingleton<Services.IdleTracker>();

            var idleShutdown = builder.Configuration["idle-shutdown"];
            if (!string.IsNullOrEmpty(idleShutdown))
            {
                var parts = idleShutdown.Split(':');
                var idleOptions = new Services.IdleShutdownOptions(
                    GraceHours: double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                    HalfLifeHours: double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                    MaxAgeHours: double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                    InitialTimeout: TimeSpan.FromHours(24));
                builder.Services.AddSingleton(idleOptions);
                builder.Services.AddHostedService<Services.IdleShutdownService>();
            }

            builder.Services.AddEndpointsOpenApi();

            var app = builder.Build();

            if (!string.IsNullOrEmpty(servedBy))
            {
                app.Use((context, next) =>
                {
                    context.Response.OnStarting(() =>
                    {
                        context.Response.Headers["Served-By"] = servedBy;
                        return Task.CompletedTask;
                    });
                    return next();
                });
            }

            app.UseResponseCompression();

#if DEBUG
            app.UseDeveloperExceptionPage();
#else
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    Console.WriteLine("Exception");
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = feature?.Error;

                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";
                        context.Response.Headers["x-manager-exception"] = exception.GetType().Name;
                    }

                    await context.Response.WriteAsync("<h1>Internal Error</h1>");
                    await context.Response.WriteAsync("<hr />");
                    await context.Response.WriteAsync("Manager " + typeof(Program).Assembly.GetName().Version.ToString());
                    await context.Response.WriteAsync("<hr />");
                    if (exception != null)
                    {
                        Console.WriteLine(exception.ToString());
                        await context.Response.WriteAsync("<pre>" + exception.ToString() + "</pre>");
                    }
                    await context.Response.WriteAsync(@"<p><a href=""javascript:window.history.back();"">Go back</a>");
                });
            });
#endif

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new ManifestEmbeddedFileProvider(typeof(HttpServer).Assembly, "wwwroot"),
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Request.QueryString.Value?.Contains("version") == true)
                    {
                        const int year = 60 * 60 * 24 * 365;
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={year},immutable";
                    }
                }
            });

            app.UseMiddleware<UserResolutionMiddleware>();
            app.MapResumableUpload();

            app.MapApi();
            app.MapEndpoints();

            app.MapOpenApi();
            app.MapScalarApiReference();

            // This is because MapFallback cannot handle file extensions (.json) - remove this code when old API is removed
            app.MapWhen(ctx => ctx.Request.Path == "/api.json" || (ctx.Request.Path.Value?.StartsWith("/api/") == true && !ctx.Request.Path.Value.StartsWith("/api/v1/")), builder =>
            {
                builder.Run(async context =>
                {
                    var handler = (HttpFramework.HttpHandler)Activator.CreateInstance(typeof(HttpHandlers.Api.Api));
                    handler.HttpContext = context;
                    handler.StringBuilder = new StringBuilder(32000);
                    context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                    await handler.ProcessRequest();
                    if (handler.StringBuilder.Length > 0)
                    {
                        if (string.IsNullOrWhiteSpace(handler.Response.ContentType)) handler.Response.ContentType = "text/html";
                        var pipe = handler.Response.BodyWriter;
                        foreach (var chunk in handler.StringBuilder.GetChunks())
                        {
                            Encoding.UTF8.GetBytes(chunk.Span, pipe);
                        }
                        await pipe.FlushAsync();
                    }
                });
            });

            app.MapFallback(async (HttpContext context, Services.IdleTracker idleTracker) =>
            {
                var path = context.Request.Path.Value.Split('/').Skip(1).FirstOrDefault() ?? string.Empty;
                if (Assembly.ContainsHttpHandler(path))
                {
                    var handlerType = Assembly.GetHttpHandlerType(path);
                    HttpFramework.HttpHandler handler = null;
                    if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value) && !context.Request.QueryString.Value.Substring(1).Split('&')[0].Contains('='))
                    {
                        handler = (HttpFramework.HttpHandler)HttpFramework.Serialization.Deserialize2(handlerType, context.Request.QueryString.Value.Substring(1).Split('&')[0]);
                    }
                    else
                    {
                        handler = (HttpFramework.HttpHandler)Activator.CreateInstance(handlerType);
                    }

                    handler.HttpContext = context;
                    handler.StringBuilder = new StringBuilder(32000);

                    context.Response.Headers["Cache-Control"] = "no-cache, no-store";

                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    context.Features.Set<System.Diagnostics.Stopwatch>(stopwatch);

                    var transaction = SentrySdk.StartTransaction(handlerType.Name, handlerType.Namespace); // We need to create it manually because we are using app.Run() instead of app.MapGet(), app.MapPost() etc.

                    await handler.ProcessRequest();

                    if (handler.StringBuilder.Length > 0)
                    {
                        if (string.IsNullOrWhiteSpace(handler.Response.ContentType)) handler.Response.ContentType = "text/html";
                        var pipe = handler.Response.BodyWriter;
                        foreach (var chunk in handler.StringBuilder.GetChunks())
                        {
                            Encoding.UTF8.GetBytes(chunk.Span, pipe);
                        }
                        await pipe.FlushAsync();
                    }

                    transaction.Finish();

                    if (handler is not HttpHandlers.Status) idleTracker.MarkActivity();
                }
                else
                {
                    context.Response.Redirect("/");
                }
            });

            using (var scope = app.Services.CreateScope())
            {
                await ApplicationData.Instance.TryUpgrade();
            }

            if (!string.IsNullOrEmpty(openApiPublishUrl) || !string.IsNullOrEmpty(openApiOutputDir))
            {
                await app.StartAsync();
                await PublishOpenApiDocumentsAsync(app, openApiPublishUrl, openApiOutputDir);
                Environment.Exit(0);
            }

            return app;
        }

        public static async Task Run(string[] args)
        {
            var app = await Build(args);
            app.Run();
        }

        private static async Task PublishOpenApiDocumentsAsync(WebApplication app, string url, string outputDir)
        {
            var documents = new Dictionary<string, string>();
            foreach (var name in MapEndpointsExtension.GetOpenApiDocumentNames())
            {
                var provider = app.Services.GetRequiredKeyedService<Microsoft.AspNetCore.OpenApi.IOpenApiDocumentProvider>(name);
                var doc = await provider.GetOpenApiDocumentAsync();
                var json = await doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
                documents[name] = json;
                Console.WriteLine($"===== {name} =====");
                Console.WriteLine(json);
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                // Only view-v1 is consumed locally (embedded into the theme skeleton); other docs are for the published site.
                if (documents.TryGetValue("get-view-v1", out var viewV1Json))
                {
                    var path = Path.Combine(outputDir, "view-v1.json");
                    await File.WriteAllTextAsync(path, viewV1Json, Encoding.UTF8);
                    Console.WriteLine($"Wrote {path}");
                }
            }

            if (string.IsNullOrEmpty(url)) return;

            var payload = JsonSerializer.Serialize(documents);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var compressed = new MemoryStream();
            using (var gzip = new System.IO.Compression.GZipStream(compressed, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: true))
            {
                await gzip.WriteAsync(payloadBytes);
            }

            using var http = new HttpClient();
            http.Timeout = TimeSpan.FromMinutes(2);
            using var content = new ByteArrayContent(compressed.ToArray());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
            content.Headers.ContentEncoding.Add("gzip");
            using var response = await http.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"PUT {url} returned {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
            }

            Console.WriteLine($"Published {documents.Count} OpenAPI document(s) to {url}");
        }

    }
}
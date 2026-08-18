using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ManagerServer.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ManagerServer.Extensions
{
    public static class ResumableUploadExtension
    {
        private static readonly string BasePath = Path.Combine(Path.GetTempPath(), "manager-uploads");

        private const long MaxFileSize = 10L * 1024 * 1024 * 1024; // 10 GB
        private const int MinChunkSize = 256 * 1024; // 256 KB
        private const int MaxChunkSize = 10 * 1024 * 1024; // 10 MB
        private const int MaxTotalChunks = 10_000;

        public static IEndpointRouteBuilder MapResumableUpload(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/upload").WithGroupName("obsolete").AddEndpointFilter(async (context, next) =>
            {
                var user = context.HttpContext.GetManagerUser();
                if (user == null)
                {
                    return Results.Json(new { error = "Authentication required" }, statusCode: 401);
                }
                return await next(context);
            });

            group.MapPost("/start", HandleStart);
            group.MapPost("/chunk", (Delegate)HandleChunk);
            group.MapPost("/complete", (Delegate)HandleComplete);
            group.MapPost("/abort", HandleAbort);

            return app;
        }

        private static IResult HandleStart(HttpContext context)
        {
            var query = context.Request.Query;

            if (!long.TryParse(query["fileSize"], out var fileSize) || fileSize <= 0)
            {
                return Results.Json(new { error = "fileSize must be greater than 0" }, statusCode: 400);
            }

            if (fileSize > MaxFileSize)
            {
                return Results.Json(new { error = $"fileSize exceeds maximum of {MaxFileSize} bytes" }, statusCode: 400);
            }

            var chunkSize = int.TryParse(query["chunkSize"], out var cs) && cs > 0 ? cs : 1024 * 1024;

            if (chunkSize < MinChunkSize || chunkSize > MaxChunkSize)
            {
                return Results.Json(new { error = $"chunkSize must be between {MinChunkSize} and {MaxChunkSize} bytes" }, statusCode: 400);
            }

            var totalChunks = (int)Math.Ceiling((double)fileSize / chunkSize);
            if (totalChunks > MaxTotalChunks)
            {
                return Results.Json(new { error = $"Upload would require {totalChunks} chunks, exceeding maximum of {MaxTotalChunks}" }, statusCode: 400);
            }

            var uploadId = Guid.NewGuid().ToString("N");
            var dir = Path.Combine(BasePath, $"{uploadId}_{totalChunks}");
            Directory.CreateDirectory(dir);

            return Results.Json(new { uploadId, chunkSize, totalChunks });
        }

        private static async Task<IResult> HandleChunk(HttpContext context)
        {
            var query = context.Request.Query;
            var uploadId = query["uploadId"].ToString();

            if (string.IsNullOrEmpty(uploadId))
            {
                return Results.Json(new { error = "uploadId is required" }, statusCode: 400);
            }

            if (!int.TryParse(query["chunkIndex"], out var chunkIndex))
            {
                return Results.Json(new { error = "chunkIndex is required" }, statusCode: 400);
            }

            var (dir, totalChunks) = FindUploadDirectory(uploadId);
            if (dir == null)
            {
                return Results.Json(new { error = "Upload session not found" }, statusCode: 404);
            }

            if (chunkIndex < 0 || chunkIndex >= totalChunks)
            {
                return Results.Json(new { error = $"chunkIndex must be between 0 and {totalChunks - 1}" }, statusCode: 400);
            }

            var chunkPath = Path.Combine(dir, $"{chunkIndex}.part");
            using (var fs = new FileStream(chunkPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await context.Request.Body.CopyToAsync(fs);
            }

            return Results.Json(new { chunkIndex, received = true });
        }

        private static async Task<IResult> HandleComplete(HttpContext context)
        {
            var uploadId = context.Request.Query["uploadId"].ToString();

            if (string.IsNullOrEmpty(uploadId))
            {
                return Results.Json(new { error = "uploadId is required" }, statusCode: 400);
            }

            var (dir, totalChunks) = FindUploadDirectory(uploadId);
            if (dir == null)
            {
                return Results.Json(new { error = "Upload session not found" }, statusCode: 404);
            }

            for (var i = 0; i < totalChunks; i++)
            {
                if (!File.Exists(Path.Combine(dir, $"{i}.part")))
                {
                    return Results.Json(new { error = $"Missing chunk {i}" }, statusCode: 400);
                }
            }

            var assembledPath = Path.Combine(dir, "assembled");
            using (var output = new FileStream(assembledPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                for (var i = 0; i < totalChunks; i++)
                {
                    var chunkPath = Path.Combine(dir, $"{i}.part");
                    using var fs = new FileStream(chunkPath, FileMode.Open, FileAccess.Read);
                    await fs.CopyToAsync(output);
                }
            }

            for (var i = 0; i < totalChunks; i++)
            {
                try { File.Delete(Path.Combine(dir, $"{i}.part")); } catch { }
            }

            return Results.Json(new { uploadId });
        }

        private static IResult HandleAbort(HttpContext context)
        {
            var uploadId = context.Request.Query["uploadId"].ToString();

            if (string.IsNullOrEmpty(uploadId))
            {
                return Results.Json(new { error = "uploadId is required" }, statusCode: 400);
            }

            var (dir, _) = FindUploadDirectory(uploadId);
            if (dir == null)
            {
                return Results.Json(new { error = "Upload session not found" }, statusCode: 404);
            }

            try { Directory.Delete(dir, true); } catch { }

            return Results.Json(new { aborted = true });
        }

        private static (string dir, int totalChunks) FindUploadDirectory(string uploadId)
        {
            if (!Directory.Exists(BasePath)) return (null, 0);

            var prefix = uploadId + "_";
            var match = Directory.GetDirectories(BasePath).FirstOrDefault(d => Path.GetFileName(d).StartsWith(prefix));
            if (match == null) return (null, 0);

            var folderName = Path.GetFileName(match);
            var underscoreIndex = folderName.IndexOf('_');
            if (underscoreIndex >= 0 && int.TryParse(folderName.Substring(underscoreIndex + 1), out var totalChunks))
            {
                return (match, totalChunks);
            }

            return (null, 0);
        }

        public static string GetUploadDirectory(string uploadId)
        {
            return FindUploadDirectory(uploadId).dir;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace HttpFramework
{
    public abstract class HttpHandler : HtmlContent
    {
        private static readonly ConcurrentDictionary<Type, string> cache = new ConcurrentDictionary<Type, string>();

        public ManagerServer.ApplicationData ApplicationData { get => HttpContext.RequestServices.GetRequiredService<ManagerServer.ApplicationData>(); }
        public HttpContext HttpContext { get; set; }
        public HttpRequest Request { get => HttpContext.Request; }
        public HttpResponse Response { get => HttpContext.Response; }
        public IFeatureCollection Features { get => HttpContext.Features; }

        public virtual Task ProcessRequest()
        {
            return Task.CompletedTask;
        }

        public static string ConvertPascalToKebabCase(Type type)
        {
            return cache.GetOrAdd(type, _ =>
            {
                var sb = new StringBuilder();
                foreach (var e in type.Name)
                {
                    if (e == '_')
                    {
                        sb.Append('.');
                    }
                    else
                    {
                        if (char.IsUpper(e) && sb.Length > 0) sb.Append('-');
                        sb.Append(char.ToLowerInvariant(e));
                    }
                }

                var value = sb.ToString();

                if (value == "default") return "/";
                else return $"/{value}";
            });
        }

        public string ToUrl()
        {
            var path = ConvertPascalToKebabCase(this.GetType());

            var buffer = new ArrayBufferWriter<byte>();
            ProtoBuf.Serializer.Serialize(buffer, this);
            if (buffer.WrittenCount > 0)
            {
                var base64 = Convert.ToBase64String(buffer.WrittenSpan);
                var sb = new StringBuilder(path.Length + base64.Length + 1);
                sb.Append(path);
                sb.Append('?');

                // Append base64 with character replacements, skip trailing '='
                for (int i = 0; i < base64.Length; i++)
                {
                    var c = base64[i];
                    if (c == '=') continue;
                    if (c == '+') sb.Append('-');
                    else if (c == '/') sb.Append('_');
                    else sb.Append(c);
                }

                return sb.ToString();
            }
            return path;
        }       
    }    
}
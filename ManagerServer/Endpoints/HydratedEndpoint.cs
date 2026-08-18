using System;
using System.IO;
using System.Linq;

namespace ManagerServer.Endpoints
{
    internal abstract class HydratedEndpoint<T> : Endpoint<T>
    {
        public sealed override T Handle()
        {
            if (Context.Request.Query.ContainsKey("q"))
            {
                var queryString = Context.Request.Query["q"].ToString();
                try
                {
                    var buffer = System.Buffers.Text.Base64Url.DecodeFromChars(queryString);
                    using (var ms = new MemoryStream(buffer))
                    {
                        ProtoBuf.Serializer.NonGeneric.Merge(ms, this);
                    }
                }
                catch (FormatException)
                {
                    throw new BadRequestException("Query string is not valid Base64Url.");
                }
                catch (EndOfStreamException)
                {
                    throw new BadRequestException("Query string payload is truncated.");
                }
                catch (ProtoException)
                {
                    throw new BadRequestException("Query string payload is not a valid request.");
                }
            }
            return HydratedHandle();
        }

        public virtual T HydratedHandle()
        {
            return default;
        }

        public override string ToUrl()
        {
            var url = Endpoint.ToUrl(GetType());
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.NonGeneric.Serialize(ms, this);
                if (ms.Length > 0)
                {
                    url += "?q=" + System.Buffers.Text.Base64Url.EncodeToString(ms.ToArray());
                }
            }
            return url;
        }
    }
}

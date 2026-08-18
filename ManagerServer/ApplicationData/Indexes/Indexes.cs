using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Indexes(IFileSystem fs)
    {
        readonly ConcurrentDictionary<Guid, byte[]> cache = new();

        string Key(Guid id) => $"Indexes/{id:N}.index";

        internal async Task WriteAsync(Guid id, Stream data)
        {
            cache.TryRemove(id, out _);
            await fs.WriteAsync(Key(id), data);
        }

        internal async Task<Stream> Open(Guid id)
        {
            if (!cache.TryGetValue(id, out var bytes))
            {
                using var stream = await fs.ReadAsync(Key(id));
                if (stream == null) return null;
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                bytes = ms.ToArray();
                cache[id] = bytes;
            }
            return new MemoryStream(bytes);
        }

        internal async Task Delete(Guid id)
        {
            cache.TryRemove(id, out _);
            await fs.DeleteAsync(Key(id));
        }

        internal void ClearCache() => cache.Clear();

        internal async Task<(int Count, long Size)> GetStats()
        {
            int count = 0;
            long size = 0;
            foreach (var key in (await fs.GetKeysAsync("Indexes/")).Where(f => f.EndsWith(".index")))
            {
                count++;
                size += await fs.GetSizeAsync(key);
            }
            return (count, size);
        }
    }
}

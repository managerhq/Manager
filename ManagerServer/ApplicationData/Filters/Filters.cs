using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Filters(IFileSystem fs)
    {
        readonly ConcurrentDictionary<Guid, Bloom> blooms = new();

        string Key(Guid id) => $"Filters/{id:N}.bloom";

        internal async Task<Bloom> GetBloomAsync(Guid id)
        {
            if (!blooms.TryGetValue(id, out var bloom))
            {
                using var stream = await fs.ReadAsync(Key(id));
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                bloom = Bloom.Load(ms.ToArray());
                blooms[id] = bloom;
            }
            return bloom;
        }

        internal async Task WriteAsync(Guid id, Stream data)
        {
            await fs.WriteAsync(Key(id), data);
        }

        internal async Task<List<Guid>> ListIds()
        {
            return (await fs.GetKeysAsync("Filters/"))
                .Where(f => f.EndsWith(".bloom"))
                .Select(f =>
                {
                    var name = f.Substring("Filters/".Length);
                    return Guid.ParseExact(name.Substring(0, name.Length - ".bloom".Length), "N");
                })
                .ToList();
        }

        internal async Task Delete(Guid id)
        {
            blooms.TryRemove(id, out _);
            await fs.DeleteAsync(Key(id));
        }

        internal void ClearCache() => blooms.Clear();

        internal async Task<(int Count, long Size)> GetStats()
        {
            int count = 0;
            long size = 0;
            foreach (var key in (await fs.GetKeysAsync("Filters/")).Where(f => f.EndsWith(".bloom")))
            {
                count++;
                size += await fs.GetSizeAsync(key);
            }
            return (count, size);
        }
    }
}

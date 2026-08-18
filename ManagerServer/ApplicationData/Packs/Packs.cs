using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Packs(IFileSystem fs)
    {

        string Key(Guid id) => $"Packs/{id:N}.tar";

        internal async Task WriteAsync(Guid id, Stream data)
        {
            await fs.WriteAsync(Key(id), data);
        }

        internal async Task<Stream> Open(Guid id) => await fs.ReadAsync(Key(id));

        internal async Task<Stream> Open(Guid id, long offset, int length) => await fs.ReadAsync(Key(id), offset, length);

        internal async Task<List<(Guid Id, long Size)>> ListAll()
        {
            var keys = await fs.GetKeysAsync("Packs/");
            var result = new List<(Guid, long)>();
            foreach (var f in keys.Where(f => f.EndsWith(".tar")))
            {
                var name = f.Substring("Packs/".Length);
                var id = Guid.ParseExact(name.Substring(0, name.Length - ".tar".Length), "N");
                var size = await fs.GetSizeAsync(f);
                result.Add((id, size));
            }
            return result;
        }

        internal async Task Delete(Guid id) => await fs.DeleteAsync(Key(id));

        internal async Task<(int Count, long Size, List<Storage.PackBucket> Buckets)> GetStats()
        {
            int count = 0;
            long size = 0;
            var byBucket = new Dictionary<int, (int Count, long Size)>();
            foreach (var key in (await fs.GetKeysAsync("Packs/")).Where(f => f.EndsWith(".tar")))
            {
                var s = await fs.GetSizeAsync(key);
                count++;
                size += s;
                var bucket = (int)Math.Log2(Math.Max(s, 1));
                byBucket.TryGetValue(bucket, out var cur);
                byBucket[bucket] = (cur.Count + 1, cur.Size + s);
            }
            var buckets = byBucket
                .OrderBy(kv => kv.Key)
                .Select(kv => new Storage.PackBucket(kv.Key, kv.Value.Count, kv.Value.Size))
                .ToList();
            return (count, size, buckets);
        }
    }
}

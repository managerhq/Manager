using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Blobs(IFileSystem fs)
    {
        static string Key(byte[] sha256)
        {
            if (sha256 == null || sha256.Length != 32) throw new ArgumentException("Expected 32 bytes");
            var hex = Convert.ToHexStringLower(sha256);
            return $"Blobs/{hex[..2]}/{hex}";
        }

        internal async Task<bool> ExistsAsync(byte[] sha256) => await fs.ExistsAsync(Key(sha256));

        internal async Task<Stream> OpenReadAsync(byte[] sha256) => await fs.ReadAsync(Key(sha256));

        internal async Task WriteAsync(byte[] sha256, Stream data)
        {
            await fs.WriteAsync(Key(sha256), data);
        }

        internal async Task<List<byte[]>> ListAllAsync()
        {
            return (await fs.GetKeysAsync("Blobs/"))
                .Select(x => x.Split('/').Last())
                .Where(x => x.Length == 64)
                .Select(x => Convert.FromHexString(x))
                .ToList();
        }

        internal async Task<long> GetFileSizeAsync(byte[] sha256) => await fs.GetSizeAsync(Key(sha256));

        internal async Task DeleteAsync(byte[] sha256) => await fs.DeleteAsync(Key(sha256));

        internal async Task<(int Count, long Size, int OversizedCount, long OversizedSize)> GetStats(long maxPackSize)
        {
            int count = 0, oversizedCount = 0;
            long size = 0, oversizedSize = 0;
            foreach (var key in await fs.GetKeysAsync("Blobs/"))
            {
                var fileSize = await fs.GetSizeAsync(key);
                if (fileSize > maxPackSize)
                {
                    oversizedCount++;
                    oversizedSize += fileSize;
                }
                else
                {
                    count++;
                    size += fileSize;
                }
            }
            return (count, size, oversizedCount, oversizedSize);
        }
    }
}

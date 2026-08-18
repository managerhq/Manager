using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Trash(IFileSystem fs)
    {
        static string Key(string name)
        {
            return $"Trash/{name}";
        }

        internal async Task<bool> ExistsAsync(string name) => await fs.ExistsAsync(Key(name));

        internal async Task<Stream> OpenReadAsync(string name) => await fs.ReadAsync(Key(name));

        internal async Task WriteAsync(string name, Stream data)
        {
            await fs.WriteAsync(Key(name), data);
        }

        internal async Task<List<string>> ListAllAsync()
        {
            return (await fs.GetKeysAsync("Trash/")).Select(k => k.Substring("Trash/".Length)).ToList();
        }

        internal async Task MoveToTrashAsync(string name, Stream data)
        {
            var destName = name;
            int index = 1;
            while (await ExistsAsync(destName))
            {
                index++;
                var ext = Path.GetExtension(name);
                var nameWithoutExt = Path.GetFileNameWithoutExtension(name);
                destName = $"{nameWithoutExt} ({index}){ext}";
            }
            await WriteAsync(destName, data);
        }

        internal async Task<long> GetFileSizeAsync(string name) => await fs.GetSizeAsync(Key(name));

        internal async Task DeleteAsync(string name) => await fs.DeleteAsync(Key(name));

        internal async Task<(int Count, long Size, int OversizedCount, long OversizedSize)> GetStats(long maxPackSize)
        {
            int count = 0, oversizedCount = 0;
            long size = 0, oversizedSize = 0;
            foreach (var key in await fs.GetKeysAsync("Trash/"))
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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer
{
    public class FileSystem(string directory) : IFileSystem
    {
        string ToPath(string key)
        {
            var path = Path.GetFullPath(Path.Combine(directory, key.Replace('/', Path.DirectorySeparatorChar)));
            if (!path.StartsWith(Path.GetFullPath(directory) + Path.DirectorySeparatorChar) && path != Path.GetFullPath(directory))
                throw new UnauthorizedAccessException($"Access denied: '{key}'");
            return path;
        }

        public Task<string[]> GetKeysAsync(string prefix)
        {
            var dir = ToPath(prefix);
            if (!Directory.Exists(dir)) return Task.FromResult<string[]>([]);
            var keys = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                .Select(f => prefix + Path.GetRelativePath(dir, f).Replace(Path.DirectorySeparatorChar, '/'))
                .ToArray();
            return Task.FromResult(keys);
        }

        public Task<bool> ExistsAsync(string key) => Task.FromResult(File.Exists(ToPath(key)));

        public Task<Stream> ReadAsync(string key, long? offset = null, int? length = null)
        {
            var path = ToPath(key);
            if (!File.Exists(path)) return Task.FromResult<Stream>(null);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (offset.HasValue && length.HasValue)
            {
                fs.Seek(offset.Value, SeekOrigin.Begin);
                return Task.FromResult<Stream>(new SubStream(fs, length.Value));
            }
            return Task.FromResult<Stream>(fs);
        }

        public async Task WriteAsync(string key, Stream stream)
        {            
            var tempPath = System.IO.Path.GetTempFileName();
            await using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }
            var path = ToPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Move(tempPath, path, overwrite: true);
        }

        public Task DeleteAsync(string key)
        {
            var path = ToPath(key);
            if (File.Exists(path)) File.Delete(path);
            return Task.CompletedTask;
        }

        public void MoveAsync(string sourceKey, string destKey)
        {
            var source = ToPath(sourceKey);
            var dest = ToPath(destKey);
            Directory.CreateDirectory(Path.GetDirectoryName(dest));
            File.Move(source, dest);
        }

        public Task<long> GetSizeAsync(string key) => Task.FromResult(new FileInfo(ToPath(key)).Length);

        public Task<string> GetFullPathAsync(string key) => Task.FromResult(ToPath(key));
    }
}

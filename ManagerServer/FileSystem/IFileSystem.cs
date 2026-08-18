using System.IO;
using System.Threading.Tasks;

namespace ManagerServer
{
    public interface IFileSystem
    {
        Task<string[]> GetKeysAsync(string prefix);
        Task<bool> ExistsAsync(string key);
        Task<Stream> ReadAsync(string key, long? offset = null, int? length = null);
        Task WriteAsync(string key, Stream stream);
        Task DeleteAsync(string key);
        Task<long> GetSizeAsync(string key);
    }
}
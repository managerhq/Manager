using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Assets(IFileSystem fs)
    {
        internal async Task<bool> ExistsAsync(string name) => await fs.ExistsAsync(name);
        internal async Task<Stream> OpenReadAsync(string name) => await fs.ReadAsync(name);
        internal async Task WriteAsync(string name, Stream data) => await fs.WriteAsync(name, data);
        internal async Task DeleteAsync(string name) => await fs.DeleteAsync(name);
    }
}

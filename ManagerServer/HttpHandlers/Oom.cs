#if DEBUG
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    internal sealed class Oom : HttpHandler
    {
        public override Task Get()
        {
            throw new OutOfMemoryException();
        }
    }
}
#endif
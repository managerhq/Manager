using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer
{
    static class TrashExtensions
    {
        internal static async Task<string[]> GetRemovedBusinesses(this ApplicationData app)
        {
            return (await app.Trash.ListAllAsync())
                .Where(k => k.EndsWith(".manager"))
                .ToArray();
        }

        internal static async Task RemoveBusiness(this ApplicationData app, string entityId)
        {
            app.Businesses.Refresh();
            if (app.Businesses.Exists(entityId))
            {
                using (var stream = await app.Businesses.OpenReadAsync(entityId))
                {
                    await app.Trash.MoveToTrashAsync($"{entityId}.manager", stream);
                }
                await app.Businesses.DeleteAsync(entityId);
                app.Businesses.Refresh();
            }
        }

        internal static async Task<bool> RestoreBusiness(this ApplicationData app, string name)
        {
            if (await app.Businesses.FileExists(Path.GetFileNameWithoutExtension(name))) return false;
            using (var stream = await app.Trash.OpenReadAsync(name))
            {
                await app.Businesses.ImportStream(Path.GetFileNameWithoutExtension(name), stream);
            }
            await app.Trash.DeleteAsync(name);
            app.Businesses.Refresh();
            return true;
        }
    }
}

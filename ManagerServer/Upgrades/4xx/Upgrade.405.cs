using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade405(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            using (var tx = objects.BeginTransaction())
            {
                tx.CreateTable<Model.Obsolete.Obsolete89.Blob2>();
                tx.Commit();
            }
            return null;
        }
    }
}
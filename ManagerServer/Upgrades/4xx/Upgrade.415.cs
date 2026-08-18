using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerServer.Model;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade415(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            using (var tx = objects.BeginTransaction())
            {
                tx.CreateTable<Model.Obsolete.Obsolete89.Blob2>();
                tx.Commit();
            }
            var hashes = objects.QueryScalars<byte[]>("SELECT Hash FROM Blobs2");
            foreach (var hash in hashes)
            {
                var blob = objects.Get<Model.Obsolete.Obsolete89.Blob2>(hash);
                await ApplicationData.Instance.Storage.WriteAsync(blob.Content);
            }
            return null;
        }
    }
}
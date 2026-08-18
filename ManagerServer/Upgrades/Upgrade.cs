using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        internal static async Task<bool> Process(string fileId, IProgress<Tuple<int, int>> progress)
        {
            var methods = typeof(Upgrade).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Where(x => x.Name.StartsWith("Upgrade")).ToArray();
            var targetSchema = methods.Select(x => int.Parse(x.Name.Substring("Upgrade".Length))).Max() + 1;

            using (var db = ApplicationData.Instance.Businesses.SQLiteConnection(fileId))
            {
                using (var tx0 = db.BeginTransaction())
                {
                    tx0.CreateTable<ManagerServer.ApplicationData.Object>();
                    tx0.Commit();
                }

                var schema = db.Single<Schema>();
                if (schema.Version == 0)
                {
                    using (var tx0 = db.BeginTransaction())
                    {
                        schema.Version = targetSchema;
                        tx0.InsertOrReplace2(schema);
                        tx0.Commit();
                    }
                    return true;
                }

                if (schema.Version > targetSchema)
                {
                    return false;
                }

                while (schema.Version < targetSchema)
                {
                    progress.Report(new Tuple<int, int>(0, 0));

                    var method = methods.Single(x => x.Name == "Upgrade" + schema.Version.ToString());
                    var upgrade = (Func<Orm.SQLiteConnection, IProgress<Tuple<int, int>>, Task<IEnumerable<Model.Object>>>)Delegate.CreateDelegate(typeof(Func<Orm.SQLiteConnection, IProgress<Tuple<int, int>>, Task<IEnumerable<Model.Object>>>), method);

                    var objectsToPut = await upgrade(db, progress);

                    using var tx = db.BeginTransaction();
                    if (objectsToPut != null)
                    {
                        foreach (var e in objectsToPut.Where(x => x is not ManagerServer.Model.Obsolete.ObsoleteSingleton))
                        {
                            tx.InsertOrReplace2(e);
                        }
                        foreach (var e in objectsToPut.Where(x => x is ManagerServer.Model.Obsolete.ObsoleteSingleton))
                        {
                            tx.Delete2(e.Key);
                        }
                    }
                    schema.Version += 1;
                    tx.InsertOrReplace2(schema);
                    tx.Commit();
                }
            }

            return true;
        }
    }
}

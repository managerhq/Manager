using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade300(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            /*
            try
            {
                using (var c = Manager.ApplicationData.Instance.Businesses.SQLiteConnection(fileId))
                {
                    var commits = c.Table<Manager.ApplicationData.Commit>().ToDictionary(x => x.Key);

                    var changes = c.QueryScalars<Guid>(@"SELECT DISTINCT ""Commit"" FROM Changes WHERE Timestamp IS NULL");
                    c.BeginTransaction();
                    foreach (var e in changes)
                    {
                        if (commits.ContainsKey(e))
                        {
                            c.Execute(@"UPDATE Changes SET User = ?, Timestamp = ? WHERE ""Commit"" = ?", commits[e].User, commits[e].Timestamp, e);
                        }
                    }
                    c.Commit();
                }
            }
            catch
            {
            }
            */
            return null;
        }
    }
}

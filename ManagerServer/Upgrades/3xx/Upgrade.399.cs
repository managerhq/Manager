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
        private static async Task<IEnumerable<Model.Object>> Upgrade399(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            if (objects.OfType<Model.Obsolete.Obsolete88.User>().Length > 0)
            {
                var administratorKey = new Guid("b79e3e24-4ea6-4e35-bcb8-849a5acea760");
                var user = objects.SingleOrDefault<Model.Obsolete.Obsolete88.User>(administratorKey);
                if (user != null)
                {
                    return [new ObsoleteSingleton() { Key = administratorKey }];
                }
            }
            return null;
        }
    }
}

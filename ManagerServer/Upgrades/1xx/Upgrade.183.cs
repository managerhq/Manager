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
        private static async Task<IEnumerable<Model.Object>> Upgrade183(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var keys = new HashSet<Guid>();
            keys.Add(new Guid("e8e8dd97-e5f9-4bb7-a5e9-390b8c56923e"));
            keys.Add(new Guid("33e4168b-d19a-47db-8f36-a729f7002a76"));
            keys.Add(new Guid("18327127-5362-4949-8f35-468dd93bc4ca"));
            keys.Add(new Guid("33065dfc-365f-47a8-8bfe-fc7ecd62545f"));

            foreach (var e in objects.OfType<ManagerServer.Model.CustomField>().Where(x => keys.Contains(x.Key)).ToArray())
            {
                e.DisplayOnView = false;
                list.Add(e);
            }

            return list;
        }
    }
}

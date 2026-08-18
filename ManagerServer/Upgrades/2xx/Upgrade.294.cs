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
        private static async Task<IEnumerable<Model.Object>> Upgrade294(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var country = new Guid("343a8633-5d10-46ca-9d20-0beed32ebab8");

            foreach (var e in objects.OfType<ManagerServer.Model.Employee>())
            {
                if (e.CustomFields == null) continue;
                if (!e.CustomFields.ContainsKey(country)) continue;
                if (e.CustomFields[country] == "Australia")
                {
                    e.CustomFields[country] = "au — Australia";
                    list.Add(e);
                }
            }

            return list;
        }
    }
}

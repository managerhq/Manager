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
        private static async Task<IEnumerable<Model.Object>> Upgrade113(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete32.ViewTemplate32>().ToArray())
            {
                if (string.IsNullOrWhiteSpace(e.Markup)) continue;
                if (!e.Markup.Contains("inventory_item")) continue;
                e.Markup = e.Markup.Replace("inventory_item", "item");
                list.Add(e);
            }
            return list;
        }
    }
}

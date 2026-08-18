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
        private static async Task<IEnumerable<Model.Object>> Upgrade248(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var o = objects.OfType<ManagerServer.Model.Obsolete.Obsolete61.Equity>().FirstOrDefault();
            if (o != null && !string.IsNullOrWhiteSpace(o.Name))
            {
                var o2 = new ManagerServer.Model.Equity() { Key = new Guid("9275ff4c-4cff-41d0-b7b5-f31c783f03d8"), Name = o.Name };
                list.Add(o2);
            }
            return list;
        }
    }
}

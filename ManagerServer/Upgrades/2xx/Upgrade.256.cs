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
        private static async Task<IEnumerable<Model.Object>> Upgrade256(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.ProductionOrder>())
            {
                if (string.IsNullOrWhiteSpace(e.Reference) && e.Obsolete_Reference.HasValue && e.Obsolete_Reference.Value != 0)
                {
                    e.Reference = e.Obsolete_Reference.ToString();
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

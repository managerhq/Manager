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
        private static async Task<IEnumerable<Model.Object>> Upgrade229(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var baseCurrency = objects.OfType<ManagerServer.Model.BaseCurrency>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BaseCurrency)));
            if (baseCurrency != null && !string.IsNullOrWhiteSpace(baseCurrency.Prefix) && baseCurrency.Prefix.Contains("$"))
            {
                baseCurrency.Prefix = "$";
                list.Add(baseCurrency);
            }
            return list;
        }
    }
}

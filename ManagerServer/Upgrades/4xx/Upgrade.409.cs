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
        private static async Task<IEnumerable<Model.Object>> Upgrade409(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var extension = objects.SingleOrDefault<CustomButton>(new Guid("1de5fa53-69a7-47e4-b265-4fff9d266c03"));
            if (extension != null)
            {
                using (var tx = objects.BeginTransaction())
                {
                    extension.Endpoint = "www.luboshasko.com/extensions/zz/theme-enhancer/";
                    tx.InsertOrReplace2(extension);
                    tx.Commit();
                }
            }
            return null;
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade410(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var extension = objects.SingleOrDefault<CustomButton>(new Guid("18638fa7-8953-43a4-866e-f254c7cd932b"));
            if (extension != null)
            {
                using (var tx = objects.BeginTransaction())
                {
                    extension.Endpoint = "www.luboshasko.com/extensions/sa/zatca-phase-1-qr-generator/";
                    tx.InsertOrReplace2(extension);
                    tx.Commit();
                }
            }
            return null;
        }
    }
}

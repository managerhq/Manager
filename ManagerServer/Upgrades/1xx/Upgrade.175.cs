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
        private static async Task<IEnumerable<Model.Object>> Upgrade175(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            return new[] { new ManagerServer.Model.Obsolete.Obsolete39.ActivationKey39() { Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete39.ActivationKey39)), Code = 288707288 } };
        }
    }
}

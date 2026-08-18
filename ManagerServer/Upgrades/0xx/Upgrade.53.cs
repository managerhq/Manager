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
        private static async Task<IEnumerable<Model.Object>> Upgrade53(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            if (objects.OfType<Model.GeneralLedgerSummary>().Any()) list.Add(new Model.Obsolete.Obsolete08.Plugin08() { Key = ManagerServer.Model.Obsolete.Obsolete08.Plugins08.GeneralLedgerSummary });
            return list;
        }
    }
}

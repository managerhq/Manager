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
        private static async Task<IEnumerable<Model.Object>> Upgrade34(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var salesInvoiceTemplates = objects.OfType<Model.Obsolete.Obsolete26.SalesInvoiceTemplate26>().Where(x => x.Key != new Guid("0b96900e-552d-4d14-a070-9086e44f188d")).ToArray();
            foreach (var e in salesInvoiceTemplates)
            {
                e.Key = new Guid("0b96900e-552d-4d14-a070-9086e44f188d");
                list.Add(e);
            }
            return list;
        }
    }
}

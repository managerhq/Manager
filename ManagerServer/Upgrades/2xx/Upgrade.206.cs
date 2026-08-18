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
        private static async Task<IEnumerable<Model.Object>> Upgrade206(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            var salesInvoice = objects.OfType<ManagerServer.Model.SalesInvoice>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesInvoice))) ?? new Model.SalesInvoice() { Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesInvoice)) };
            salesInvoice.AutomaticReference = true;
            list.Add(salesInvoice);

            return list;
        }
    }
}

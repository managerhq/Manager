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
        private static async Task<IEnumerable<Model.Object>> Upgrade31(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var salesInvoices = objects.OfType<Model.SalesInvoice>().Where(x => x.Obsolete_Lines != null & x.Obsolete_Lines.Sum(y => y.Amount) < 0).ToArray();
            foreach (var e in salesInvoices)
            {
                foreach (var e2 in e.Obsolete_Lines) e2.Amount = e2.Amount * -1;
                list.Add(e);
            }
            return list;
        }
    }
}

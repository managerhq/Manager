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
        private static async Task<IEnumerable<Model.Object>> Upgrade234(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Obsolete_ConversionBalance != 0m).ToArray())
            {
                e.Obsolete_PartialPayment = true;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.Obsolete_ConversionBalance != 0m).ToArray())
            {
                e.Obsolete_PartialPayment = true;
                list.Add(e);
            }
            return list;
        }
    }
}

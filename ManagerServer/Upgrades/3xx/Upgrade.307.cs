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
        private static async Task<IEnumerable<Model.Object>> Upgrade307(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var purchaseOrders = objects.OfType<ManagerServer.Model.PurchaseOrder>().Where(x => !string.IsNullOrWhiteSpace(x.Reference)).GroupBy(x => x.Reference).ToDictionary(x => x.Key, x => x.ToArray());
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.OrderNumber) && x.PurchaseOrder == null).ToArray())
            {
                if (purchaseOrders.ContainsKey(e.OrderNumber))
                {
                    var purchaseOrder = purchaseOrders[e.OrderNumber].FirstOrDefault(x => x.Supplier == e.Supplier);
                    if (purchaseOrder != null)
                    {
                        e.PurchaseOrder = purchaseOrder.Key;
                        list.Add(e);
                    }
                }
            }

            return list;
        }
    }
}

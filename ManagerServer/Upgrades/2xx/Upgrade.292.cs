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
        private static async Task<IEnumerable<Model.Object>> Upgrade292(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<Customer>().Where(x => x.Obsolete_CustomerPortal))
            {
                list.Add(new CustomerPortal()
                {
                    Key = Guid.CreateVersion7(),
                    Customer = e.Key,
                    DeliveryNotes = true,
                    CreditNotes = true,
                    SalesInvoices = true,
                    SalesOrders = true,
                    SalesQuotes = true
                });
            }
            return list;
        }
    }
}

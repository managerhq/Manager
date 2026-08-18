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
        private static async Task<IEnumerable<Model.Object>> Upgrade107(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var customer = new ManagerServer.Model.Customer() { Key = new Guid("07c0eeac-6eba-4a77-a466-2d85c5e531c2"), Name = "Unspecified customer" };
            foreach (var e in objects.OfType<Model.SalesInvoice>().Where(x => !x.Customer.HasValue).ToArray())
            {
                e.Customer = customer.Key;
                list.Add(e);
                if (!list.Contains(customer)) list.Add(customer);
            }
            var supplier = new ManagerServer.Model.Supplier() { Key = new Guid("831b8ea8-178b-49c8-b691-648af7cd1fa5"), Name = "Unspecified supplier" };
            foreach (var e in objects.OfType<Model.PurchaseInvoice>().Where(x => !x.Supplier.HasValue).ToArray())
            {
                e.Supplier = supplier.Key;
                list.Add(e);
                if (!list.Contains(supplier)) list.Add(supplier);
            }
            return list;
        }
    }
}

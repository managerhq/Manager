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
        private static async Task<IEnumerable<Model.Object>> Upgrade240(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment>().Where(x => x.Obsolete_CustomerOrSupplier.HasValue))
            {
                var customer = objects.SingleOrDefault<Customer>(e.Obsolete_CustomerOrSupplier);
                var supplier = objects.SingleOrDefault<Supplier>(e.Obsolete_CustomerOrSupplier);
                if (customer != null)
                {
                    e.PayerPayeeType = PayerPayeeType.Customer;
                    e.Customer = customer.Key;
                    list.Add(e);
                }
                if (supplier != null)
                {
                    e.PayerPayeeType = PayerPayeeType.Supplier;
                    e.Supplier = supplier.Key;
                    list.Add(e);
                }
            }
            var customField = new CustomField() { Name = Strings.Contact, Key = new Guid("436f269c-126f-4055-847b-b8d146b7e1e8"), Type = CustomFieldStyle.SingleLineText, Size = CustomFieldSize.Medium };
            list.Add(customField); // This is a way to actually remove custom field by omitting type
            return list;
        }
    }
}

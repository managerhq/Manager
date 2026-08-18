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
        private static async Task<IEnumerable<Model.Object>> Upgrade141(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete25.SalesInvoiceItem25>().Where(x => x.AccountID == ManagerServer.Model.Master.AccountKeys.InventorySales).ToArray())
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete24.SalesInvoiceItem24()
                {
                    Key = e.Key,
                    AccountID = e.AccountID,
                    Code = e.Code,
                    CustomFields = e.CustomFields,
                    Description = e.Description,
                    Discount = e.Discount,
                    Inactive = e.Inactive,
                    Name = e.Name,
                    Obsolete_InventoryItem = e.Obsolete_InventoryItem,
                    TaxCode = e.TaxCode,
                    TrackingCode = e.TrackingCode,
                    UnitPrice = e.UnitPrice
                });
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete25.PurchaseInvoiceItem25>().Where(x => x.AccountID == ManagerServer.Model.Master.AccountKeys.InventoryOnHand).ToArray())
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete24.PurchaseInvoiceItem24()
                {
                    Key = e.Key,
                    AccountID = e.AccountID,
                    Code = e.Code,
                    CustomFields = e.CustomFields,
                    Description = e.Description,
                    Inactive = e.Inactive,
                    Name = e.Name,
                    Obsolete_InventoryItem = e.Obsolete_InventoryItem,
                    TaxCode = e.TaxCode,
                    TrackingCode = e.TrackingCode,
                    UnitPrice = e.UnitPrice
                });
            }
            return list;
        }
    }
}

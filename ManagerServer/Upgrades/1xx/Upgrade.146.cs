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
        private static async Task<IEnumerable<Model.Object>> Upgrade146(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete25.SalesInvoiceItem25>().ToArray())
            {
                list.Add(new ManagerServer.Model.NonInventoryItem()
                {
                    Code = e.Code,
                    CustomFields = e.CustomFields,
                    DefaultLineDescription = e.Description,
                    Inactive = e.Inactive,
                    WhenSold = e.AccountID,
                    DefaultSalesUnitPrice = e.UnitPrice ?? 0m,
                    DefaultTaxCode = e.TaxCode,
                    Obsolete_Division = e.TrackingCode,
                    Name = e.Name,
                    Key = e.Key
                });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete25.PurchaseInvoiceItem25>().ToArray())
            {
                list.Add(new ManagerServer.Model.NonInventoryItem()
                {
                    Code = e.Code,
                    CustomFields = e.CustomFields,
                    DefaultLineDescription = e.Description,
                    Inactive = e.Inactive,
                    WhenPurchased = e.AccountID,
                    DefaultPurchaseUnitPrice = e.UnitPrice ?? 0m,
                    Obsolete_PurchaseTaxCode = e.TaxCode,
                    Obsolete_Division = e.TrackingCode,
                    Name = e.Name,
                    Key = e.Key
                });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete25.SalesInvoiceItem25)) || x.Obsolete_FormType == Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete25.PurchaseInvoiceItem25))).ToArray())
            {
                e.Obsolete_FormType = Model.Object.GetGuidByType(typeof(ManagerServer.Model.NonInventoryItem));
                list.Add(e);
            }

            return list;
        }
    }
}

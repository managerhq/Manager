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
        private static async Task<IEnumerable<Model.Object>> Upgrade70(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var generalSettings = objects.OfType<Model.Obsolete.Obsolete11.GeneralSettings11>().SingleOrDefault(x => x.Key == Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete11.GeneralSettings11)));
            if (generalSettings != null)
            {
                list.Add(new Model.Tabs() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Tabs)), BankAndCashAccounts = generalSettings.CashAccounts, SalesInvoices = generalSettings.SalesInvoices, Customers = generalSettings.SalesInvoices, BillableTime = generalSettings.Jobs, CreditNotes = generalSettings.CreditNotes, DeliveryNotes = generalSettings.DeliveryNotes, ExpenseClaims = generalSettings.ExpenseClaims, FixedAssets = generalSettings.FixedAssets, InventoryItems = generalSettings.InventoryItems, PurchaseInvoices = generalSettings.PurchaseInvoices, PurchaseOrders = generalSettings.PurchaseOrders, SalesQuotes = generalSettings.SalesQuotes, Suppliers = generalSettings.PurchaseInvoices });
            }
            return list;
        }
    }
}

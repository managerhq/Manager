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
        private static async Task<IEnumerable<Model.Object>> Upgrade58(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var plugins = objects.OfType<Model.Obsolete.Obsolete08.Plugin08>().ToDictionary(x => x.Key);
            var generalSettings = new Model.Obsolete.Obsolete11.GeneralSettings11() { Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete11.GeneralSettings11)) };
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.BankAccounts)) generalSettings.BankAccounts = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.CashAccounts)) generalSettings.CashAccounts = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.CreditNotes)) generalSettings.CreditNotes = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.CustomerStatements)) generalSettings.CustomerStatements = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.ExpenseClaims)) generalSettings.ExpenseClaims = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.GeneralLedgerSummary)) generalSettings.GeneralLedgerSummary = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.InvoiceLogo)) generalSettings.BusinessLogo = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.PurchaseInvoices)) generalSettings.PurchaseInvoices = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.PurchaseOrders)) generalSettings.PurchaseOrders = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.SalesInvoiceItems)) generalSettings.SalesInvoiceItems = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.SalesInvoices)) generalSettings.SalesInvoices = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.SalesQuotes)) generalSettings.SalesQuotes = true;
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.TaxCodes)) generalSettings.TaxCodes = true;
            list.Add(generalSettings);
            return list;
        }
    }
}

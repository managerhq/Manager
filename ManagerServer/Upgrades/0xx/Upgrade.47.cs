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
        private static async Task<IEnumerable<Model.Object>> Upgrade47(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var setup = objects.OfType<Model.Obsolete.Obsolete06.GeneralSettings06>().SingleOrDefault(x => x.Key == new Guid("e1cf015a-89af-412f-80a2-c9b98d969cd1"));
            var list2 = new List<Guid>();

            list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.ReportingOnCashBasis);

            if (setup != null)
            {
                if (setup.AccountsPayable == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.PurchaseInvoices);
                if (setup.BankAccounts == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.BankAccounts);
                if (setup.CashAccounts == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.CashAccounts);
                if (setup.OutOfPocketExpenses == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.ExpenseClaims);
                if (setup.SalesInvoiceItems == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.SalesInvoiceItems);
                if (setup.SalesInvoices == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.SalesInvoices);
                if (setup.SalesInvoices == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.InvoiceLogo);
                if (setup.SalesQuotes == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.SalesQuotes);
                if (setup.TaxCodes == true) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.TaxCodes);
            }

            if (objects.OfType<Model.Obsolete.Obsolete22.BankAccount22>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.BankAccounts);
            if (objects.OfType<Model.Obsolete.Obsolete22.CashAccount22>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.CashAccounts);
            if (objects.OfType<Model.SalesInvoice>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.SalesInvoices);
            if (objects.OfType<Model.SalesInvoice>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.InvoiceLogo);
            if (objects.OfType<Model.PurchaseInvoice>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.PurchaseInvoices);
            if (objects.OfType<Model.SalesQuote>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.SalesQuotes);
            if (objects.OfType<Model.Obsolete.Obsolete07.TaxCode07>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.TaxCodes);
            if (!objects.OfType<Model.Obsolete.Obsolete18.GeneralLedgerAccount18>().Any()) list2.Add(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.SampleChartOfAccounts);

            list2 = list2.Distinct().ToList();
            var plugins = objects.OfType<Model.Obsolete.Obsolete08.Plugin08>().ToDictionary(x => x.Key);
            foreach (var e in list2.ToArray())
            {
                if (plugins.ContainsKey(e)) list2.Remove(e);
            }
            foreach (var e in list2)
            {
                list.Add(new Model.Obsolete.Obsolete08.Plugin08() { Key = e });
            }
            return list.ToArray();
        }
    }
}

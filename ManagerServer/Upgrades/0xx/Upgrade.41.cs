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
        private static async Task<IEnumerable<Model.Object>> Upgrade41(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            var customers = objects.OfType<Model.Customer>().ToDictionary(x => x.Key);
            var salesInvoices = objects.OfType<Model.SalesInvoice>().Where(x => x.Obsolete_IsCashSale).ToArray();
            foreach (var e in salesInvoices)
            {
                var receipt = new Model.Obsolete.Obsolete33.Receipt33();
                receipt.Date = e.IssueDate;
                receipt.DebitAccount = e.Obsolete_CashSaleDebitAccount;
                receipt.Key = Guid.CreateVersion7();
                if (e.Customer.HasValue && customers.ContainsKey(e.Customer.Value)) receipt.Payer = customers[e.Customer.Value].Name;
                var totalAmount = e.Obsolete_Lines.Sum(x => x.Amount);
                receipt.Lines = new Model.Obsolete.Obsolete76.TransactionLine[1] { new Model.Obsolete.Obsolete76.TransactionLine() { Account = Model.Master.AccountKeys.AccountsReceivable, Obsolete_SalesInvoice = e.Key, Amount = totalAmount } };
                list.Add(receipt);

                e.Obsolete_IsCashSale = false;
                list.Add(e);
            }
            return list;
        }
    }
}

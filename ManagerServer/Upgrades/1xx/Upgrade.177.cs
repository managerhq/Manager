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
        private static async Task<IEnumerable<Model.Object>> Upgrade177(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var bankPayments = objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankPayment42>().ToArray();
            foreach (var e in bankPayments) e.AmountsIncludeTax = true;
            var bankReceipts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42>().ToArray();
            foreach (var e in bankReceipts) e.AmountsIncludeTax = true;
            var cashPayments = objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashPayment43>().ToArray();
            foreach (var e in cashPayments) e.AmountsIncludeTax = true;
            var cashReceipts = objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43>().ToArray();
            foreach (var e in cashReceipts) e.AmountsIncludeTax = true;

            var list = new List<ManagerServer.Model.Object>();
            list.AddRange(bankPayments);
            list.AddRange(bankReceipts);
            list.AddRange(cashPayments);
            list.AddRange(cashReceipts);
            return list;
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade118(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var bankAccounts = objects.OfType<Model.Obsolete.Obsolete22.BankAccount22>().ToDictionary(x => x.Key);
            var cashAccounts = objects.OfType<Model.Obsolete.Obsolete22.CashAccount22>().ToDictionary(x => x.Key);

            var miscCashAccount = new Guid("eea46e8f-727d-49e7-81a9-1d264fe614ea");

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().ToArray())
            {
                if (e.DebitAccount.HasValue && e.DebitAccount.Value == miscCashAccount) continue;
                if (e.DebitAccount.HasValue && bankAccounts.ContainsKey(e.DebitAccount.Value)) continue;
                if (e.DebitAccount.HasValue && cashAccounts.ContainsKey(e.DebitAccount.Value)) continue;

                e.DebitAccount = miscCashAccount;
                list.Add(e);
            }

            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().ToArray())
            {
                if (e.CreditAccount.HasValue && e.CreditAccount.Value == miscCashAccount) continue;
                if (e.CreditAccount.HasValue && bankAccounts.ContainsKey(e.CreditAccount.Value)) continue;
                if (e.CreditAccount.HasValue && cashAccounts.ContainsKey(e.CreditAccount.Value)) continue;

                e.CreditAccount = miscCashAccount;
                list.Add(e);
            }

            if (objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Receipt33>().Any(x => x.DebitAccount == miscCashAccount) || objects.OfType<ManagerServer.Model.Obsolete.Obsolete33.Payment33>().Any(x => x.CreditAccount == miscCashAccount))
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete22.CashAccount22() { Key = miscCashAccount, Name = "Miscellaneous" });
            }
            return list;
        }
    }
}

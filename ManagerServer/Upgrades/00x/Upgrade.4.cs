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
        private static async Task<IEnumerable<Model.Object>> Upgrade4(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var cashbookAccounts = new HashSet<Guid>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete02.Receipt02>().Where(x => x.DebitAccount.HasValue).Select(x => x.DebitAccount.Value).Distinct())
            {
                if (!cashbookAccounts.Contains(e)) cashbookAccounts.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.Payment02>().Where(x => x.CreditAccount.HasValue).Select(x => x.CreditAccount.Value).Distinct())
            {
                if (!cashbookAccounts.Contains(e)) cashbookAccounts.Add(e);
            }

            var list = new List<Model.Object>();
            var generalLedgerAccounts = objects.OfType<Model.Obsolete.Obsolete01.GeneralLedgerAccount01>().ToArray();
            foreach (var e in generalLedgerAccounts)
            {
                if (cashbookAccounts.Contains(e.Key))
                {
                    list.Add(new Model.Obsolete.Obsolete22.BankAccount22()
                    {
                        Key = e.Key,
                        Name = e.Name
                    });
                }
                else
                {
                    list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18()
                    {
                        Key = e.Key,
                        Name = e.Name,
                        Category = e.Category
                    });
                }
            }
            return list.ToArray();
        }
    }
}

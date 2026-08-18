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
        private static async Task<IEnumerable<Model.Object>> Upgrade380(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var bankAccounts = new HashSet<Guid>(objects.OfType<BankOrCashAccount>().Where(x => !x.CanHavePendingTransactions).Select(x => x.Key));

            foreach (var e in objects.OfType<InterAccountTransfer>().ToArray())
            {
                if (e.ReceivedIn.HasValue)
                {
                    if (bankAccounts.Contains(e.ReceivedIn.Value))
                    {
                        if (e.DebitClearStatus == BankAccountClearStatus.OnALaterDate && !e.DebitClearDate.HasValue)
                        {
                            e.DebitClearDate = e.Date;
                            list.Add(e);
                        }
                    }
                }

                if (e.PaidFrom.HasValue)
                {
                    if (bankAccounts.Contains(e.PaidFrom.Value))
                    {
                        if (e.CreditClearStatus == BankAccountClearStatus.OnALaterDate && !e.CreditClearDate.HasValue)
                        {
                            e.CreditClearDate = e.Date;
                            list.Add(e);
                        }
                    }
                }
            }

            return list.Distinct();
        }
    }
}

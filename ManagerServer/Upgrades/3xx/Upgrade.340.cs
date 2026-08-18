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
        private static async Task<IEnumerable<Model.Object>> Upgrade340(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.InterAccountTransfer>())
            {
                if (e.Obsolete_CreditClearStatus == BankClearStatus.Pending)
                {
                    e.CreditClearStatus = BankAccountClearStatus.OnALaterDate;
                    e.Obsolete_CreditBankClearDate = e.CreditClearDate;
                    e.CreditClearDate = null;
                    list.Add(e);
                }

                if (e.Obsolete_CreditClearStatus == BankClearStatus.Cleared && e.CreditClearDate.HasValue && e.CreditClearDate.Value != e.Date)
                {
                    e.CreditClearStatus = BankAccountClearStatus.OnALaterDate;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

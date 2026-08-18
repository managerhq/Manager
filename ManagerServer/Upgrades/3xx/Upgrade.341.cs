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
        private static async Task<IEnumerable<Model.Object>> Upgrade341(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.InterAccountTransfer>())
            {
                if (e.Obsolete_DebitClearStatus == BankClearStatus.Pending)
                {
                    e.DebitClearStatus = BankAccountClearStatus.OnALaterDate;
                    e.Obsolete_DebitBankClearDate = e.DebitClearDate;
                    e.DebitClearDate = null;
                    list.Add(e);
                }

                if (e.Obsolete_DebitClearStatus == BankClearStatus.Cleared && e.DebitClearDate.HasValue && e.DebitClearDate.Value != e.Date)
                {
                    e.DebitClearStatus = BankAccountClearStatus.OnALaterDate;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

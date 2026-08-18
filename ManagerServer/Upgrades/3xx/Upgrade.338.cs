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
        private static async Task<IEnumerable<Model.Object>> Upgrade338(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Receipt>())
            {
                if (e.Obsolete_Status == BankClearStatus.Pending)
                {
                    e.Cleared = BankAccountClearStatus.OnALaterDate;
                    e.Obsolete_BankClearDate = e.BankClearDate;
                    e.BankClearDate = null;
                    list.Add(e);
                }

                if (e.Obsolete_Status == BankClearStatus.Cleared && e.BankClearDate.HasValue && e.BankClearDate.Value != e.Date)
                {
                    e.Cleared = BankAccountClearStatus.OnALaterDate;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

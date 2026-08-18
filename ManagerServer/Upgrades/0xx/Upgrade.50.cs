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
        private static async Task<IEnumerable<Model.Object>> Upgrade50(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete07.CashPayment07>().ToArray())
            {
                var o = new Model.Obsolete.Obsolete33.Payment33();
                o.Date = e.Date;
                o.CreditAccount = e.CashAccount;
                o.Description = e.Description;
                o.Key = e.Key;
                o.Lines = e.Lines;
                o.Payee = e.Payee;
                o.Reference = e.Reference;
                list.Add(o);
            }
            return list;
        }
    }
}

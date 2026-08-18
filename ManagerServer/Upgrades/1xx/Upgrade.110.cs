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
        private static async Task<IEnumerable<Model.Object>> Upgrade110(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var customExchangeRates = objects.OfType<ManagerServer.Model.Obsolete.Obsolete14.TransactionExchangeRate14>().ToArray();
            foreach (var e in objects.OfType<Model.InterAccountTransfer>().ToArray())
            {
                e.CreditAmount = e.Obsolete_Amount;
                e.DebitAmount = e.Obsolete_Amount;
                if (e.PaidFrom.HasValue && customExchangeRates.Any())
                {
                    var o = customExchangeRates.SingleOrDefault(x => x.Transaction == e.Key && x.Account == e.PaidFrom.Value);
                    if (o != null)
                    {
                        e.CreditAmount = Math.Round(e.CreditAmount * o.ExchangeRate, 2);
                    }
                }
                list.Add(e);
            }
            return list;
        }
    }
}

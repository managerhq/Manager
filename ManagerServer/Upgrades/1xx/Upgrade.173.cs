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
        private static async Task<IEnumerable<Model.Object>> Upgrade173(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete35.ExchangeRates35>().ToArray())
            {
                if (e.Rates != null)
                {
                    foreach (var e2 in e.Rates)
                    {
                        if (e2.Rate.HasValue && e2.Rate.Value != 0m)
                        {
                            list.Add(new ManagerServer.Model.ExchangeRate() { Key = Guid.CreateVersion7(), Currency = e2.Currency, Date = e.Date, Obsolete_BaseRate = e2.Rate ?? 0m });
                        }
                    }
                }
            }
            return list.ToArray();
        }
    }
}

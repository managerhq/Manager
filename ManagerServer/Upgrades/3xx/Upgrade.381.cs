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
        private static async Task<IEnumerable<Model.Object>> Upgrade381(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<Investment>())
            {
                if (e.Obsolete_MarketPrice > 0m)
                {
                    list.Add(new InvestmentMarketPrice()
                    {
                        Date = DateTime.Today,
                        Investment = e.Key,
                        Currency = e.Obsolete_Currency,
                        MarketPrice = e.Obsolete_MarketPrice
                    });
                }
            }
            return list;
        }
    }
}

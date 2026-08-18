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
        private static async Task<IEnumerable<Model.Object>> Upgrade154(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var baseCurrency = objects.OfType<ManagerServer.Model.BaseCurrency>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BaseCurrency)));
            if (baseCurrency != null && baseCurrency.Obsolete_Currency.HasValue)
            {
                foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetAccount>().ToArray())
                {
                    if (e.Obsolete_ControlAccount) continue;
                    if (e.Obsolete_Currency == null) continue;
                    if (e.Obsolete_Currency.Value == baseCurrency.Obsolete_Currency) continue;

                    var specialAccount = new ManagerServer.Model.SpecialAccount() { Obsolete_BalanceSheetAccount = e, Currency = e.Obsolete_Currency.Value, Key = e.Key, Obsolete_HasStartingBalance = e.Obsolete_HasStartingBalance, Name = e.Name, TaxCode = e.DefaultTaxCode, Obsolete_StartingBalance2 = e.Obsolete_StartingBalance2, Obsolete_StartingBalanceType2 = e.Obsolete_StartingBalanceType2 };
                    list.Add(specialAccount);
                }
            }
            return list;
        }
    }
}

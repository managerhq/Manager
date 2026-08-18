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
        private static async Task<IEnumerable<Model.Object>> Upgrade313(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>())
            {
                if (e.Obsolete_FlatRate && e.Obsolete_FlatRateRate >= 0m && e.TaxRate == TaxRate.CustomRate && e.Type == TaxRateType.SingleRate)
                {
                    var multiplier = (100 + e.Rate) / 100;
                    var flatRate = e.Obsolete_FlatRateRate * multiplier;
                    var flatRateDiscount = flatRate - e.Rate;

                    e.Type = TaxRateType.MultipleRates;
                    e.Components = new TaxCode.Component[2]
                    {
                        new TaxCode.Component() { Name = e.Name, ComponentAccount = e.Account, ComponentRate = e.Rate },
                        new TaxCode.Component() { ComponentAccount = e.Account, ComponentRate = flatRateDiscount }
                    };

                    list.Add(e);
                }
            }
            return list;
        }
    }
}

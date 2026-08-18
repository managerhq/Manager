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
        private static async Task<IEnumerable<Model.Object>> Upgrade85(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete13.CapitalControlAccount13>().Where(x => x.Key == new Guid("910bd356-43fc-456f-915e-4ac3615c0ead")).ToArray())
            {
                list.Add(new Model.Obsolete.Obsolete18.ControlAccount18() { Key = Model.Master.AccountKeys.CapitalAccounts, Category = e.Category, Name = e.Name });
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete13.RetainedEarningsConversionBalance13>().Where(x => x.Key == new Guid("01c00313-4790-451e-ae05-1ad6fc6fa476")).ToArray())
            {
                list.Add(new Model.Obsolete.Obsolete18.ControlAccount18() { Key = Model.Master.AccountKeys.RetainedEarnings, StartingBalance = e.OpeningBalance, StartingBalanceType = e.OpeningBalanceType });
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete13.TaxPayableConversionBalance13>().Where(x => x.Key == new Guid("89b898c8-d5f1-4cff-9a56-93120a92c89e")).ToArray())
            {
                list.Add(new Model.Obsolete.Obsolete18.ControlAccount18() { Key = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71"), StartingBalance = e.OpeningBalance, StartingBalanceType = e.OpeningBalanceType });
            }
            return list;
        }
    }
}

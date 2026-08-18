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
        private static async Task<IEnumerable<Model.Object>> Upgrade86(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.ProfitAndLossStatement>().ToArray())
            {
                e.Periods = new Model.ProfitAndLossStatement.Period[] { new Model.ProfitAndLossStatement.Period() { FromDate = e.Obsolete_From, ToDate = e.Obsolete_To ?? DateTime.Today, Division = e.Obsolete_TrackingCode } };
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.BalanceSheet>().ToArray())
            {
                e.Periods = new Model.BalanceSheet.Period[] { new Model.BalanceSheet.Period() { Date = e.Obsolete_Date } };
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.StatementOfChangesInEquity>().ToArray())
            {
                e.Periods = new Model.StatementOfChangesInEquity.Period[] { new Model.StatementOfChangesInEquity.Period() { FromDate = e.Obsolete_From, ToDate = e.Obsolete_To } };
                list.Add(e);
            }
            foreach (var e in objects.OfType<Model.TrialBalance>().ToArray())
            {
                e.Periods = new Model.TrialBalance.Period[] { new Model.TrialBalance.Period() { FromDate = e.Obsolete_From, ToDate = e.Obsolete_To ?? DateTime.Today } };
                list.Add(e);
            }
            return list;
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade222(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();

            var wagesAndSalaries = new ManagerServer.Model.ProfitAndLossStatementAccount() { Key = new Guid("2be9dcbd-4c7c-40f0-9e7f-ceb3352740c1"), Name = Strings.WagesAndSalaries, Group = new Guid("fd003045-876e-439e-b923-1904453f5c30") };
            var wagesAndSalariesBuiltIn = objects.OfType<ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount>().SingleOrDefault(x => x.Key == wagesAndSalaries.Key);
            if (wagesAndSalariesBuiltIn != null)
            {
                if (!string.IsNullOrWhiteSpace(wagesAndSalariesBuiltIn.Name)) wagesAndSalaries.Name = wagesAndSalariesBuiltIn.Name;
                if (wagesAndSalariesBuiltIn.Obsolete_Code.HasValue) wagesAndSalaries.Code = wagesAndSalariesBuiltIn.Code;
                if (wagesAndSalariesBuiltIn.Group.HasValue) wagesAndSalaries.Group = wagesAndSalariesBuiltIn.Group;
            }

            var taxLiabilities = new ManagerServer.Model.BalanceSheetAccount() { Key = new Guid("8a3514e3-3bac-43b8-9521-21fc59e90287"), Name = Strings.PayrollLiabilities, Group = new Guid("ed5a19f6-12c5-45cc-b4b7-4e79f7ef50bc") };
            var taxLiabilitiesBuiltIn = objects.OfType<ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount>().SingleOrDefault(x => x.Key == taxLiabilities.Key);
            if (taxLiabilitiesBuiltIn != null)
            {
                if (!string.IsNullOrWhiteSpace(taxLiabilitiesBuiltIn.Name)) taxLiabilities.Name = taxLiabilitiesBuiltIn.Name;
                if (taxLiabilitiesBuiltIn.Obsolete_Code.HasValue) taxLiabilities.Code = taxLiabilitiesBuiltIn.Code;
                if (taxLiabilitiesBuiltIn.Group.HasValue) taxLiabilities.Group = taxLiabilitiesBuiltIn.Group;
                taxLiabilities.Obsolete_StartingBalance2 = taxLiabilitiesBuiltIn.Obsolete_StartingBalance2;
                taxLiabilities.Obsolete_StartingBalanceType2 = taxLiabilitiesBuiltIn.Obsolete_StartingBalanceType;
            }

            foreach (var e in objects.OfType<ManagerServer.Model.PayslipEarningsItem>().ToArray())
            {
                if (!e.ExpenseAccount.HasValue)
                {
                    e.ExpenseAccount = wagesAndSalaries.Key;
                    if (!list.Contains(wagesAndSalaries)) list.Add(wagesAndSalaries);
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.PayslipDeductionItem>().ToArray())
            {
                if (!e.Account.HasValue)
                {
                    e.Account = taxLiabilities.Key;
                    if (!list.Contains(taxLiabilities)) list.Add(taxLiabilities);
                    list.Add(e);
                }
            }

            foreach (var e in objects.OfType<ManagerServer.Model.PayslipContributionItem>().ToArray())
            {
                if (!e.LiabilityAccount.HasValue)
                {
                    e.LiabilityAccount = taxLiabilities.Key;
                    if (!list.Contains(taxLiabilities)) list.Add(taxLiabilities);
                    list.Add(e);
                }
                if (!e.ExpenseAccount.HasValue)
                {
                    e.ExpenseAccount = wagesAndSalaries.Key;
                    if (!list.Contains(wagesAndSalaries)) list.Add(wagesAndSalaries);
                    if (!list.Contains(e)) list.Add(e);
                }
            }

            return list;
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade97(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            if (!objects.OfType<Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18>().Any())
            {
                list.Add(new Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18() { Key = Guid.CreateVersion7(), Name = "Revenue", Position = 1 });
                list.Add(new Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18() { Key = Guid.CreateVersion7(), Name = "Direct Expenses", Position = 2 });
                list.Add(new Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18() { Key = Guid.CreateVersion7(), Name = "Operating Expenses", Position = 3 });
                list.Add(new Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18() { Key = Guid.CreateVersion7(), Name = "Other Income", Position = 4 });
                list.Add(new Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18() { Key = Guid.CreateVersion7(), Name = "Other Expenses", Position = 5 });
            }

            if (!objects.OfType<Model.Obsolete.Obsolete18.ClassifiedBalanceSheetAssetGroup18>().Any())
            {
                list.Add(new Model.Obsolete.Obsolete18.ClassifiedBalanceSheetAssetGroup18() { Key = Guid.CreateVersion7(), Name = "Current Assets", Position = 1 });
                list.Add(new Model.Obsolete.Obsolete18.ClassifiedBalanceSheetAssetGroup18() { Key = Guid.CreateVersion7(), Name = "Non-current Assets", Position = 2 });
            }
            if (!objects.OfType<Model.Obsolete.Obsolete18.ClassifiedBalanceSheetLiabilityGroup18>().Any())
            {
                list.Add(new Model.Obsolete.Obsolete18.ClassifiedBalanceSheetLiabilityGroup18() { Key = Guid.CreateVersion7(), Name = "Current Liabilities", Position = 1 });
                list.Add(new Model.Obsolete.Obsolete18.ClassifiedBalanceSheetLiabilityGroup18() { Key = Guid.CreateVersion7(), Name = "Non-current Liabilities", Position = 2 });
            }
            return list;
        }
    }
}

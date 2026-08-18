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
        private static async Task<IEnumerable<Model.Object>> Upgrade98(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            if (!objects.OfType<Model.Obsolete.Obsolete18.MultiStepIncomeStatementTotal18>().Any())
            {
                var index = objects.OfType<Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18>().Where(x => !string.IsNullOrWhiteSpace(x.Obsolete_Total)).Count();
                Guid? lastTotal = null;
                foreach (var e in objects.OfType<Model.Obsolete.Obsolete18.MultiStepIncomeStatementGroup18>().OrderByDescending(x => x.Position ?? 0).ToArray())
                {
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Total))
                    {
                        lastTotal = Guid.CreateVersion7();
                        list.Add(new Model.Obsolete.Obsolete18.MultiStepIncomeStatementTotal18() { Key = lastTotal.Value, Name = e.Obsolete_Total, Position = index });
                        index--;
                    }
                    e.MultiStepIncomeStatementTotal = lastTotal;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

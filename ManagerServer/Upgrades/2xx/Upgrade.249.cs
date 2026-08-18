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
        private static async Task<IEnumerable<Model.Object>> Upgrade249(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.ProfitAndLossStatementGroup>())
            {
                if (e.Group.HasValue)
                {
                    e.Type = ProfitAndLossStatementGroupType.SubgroupOf;
                    list.Add(e);
                }
                else if (e.Obsolete_ExpenseGroup)
                {
                    e.Type = ProfitAndLossStatementGroupType.ExpenseGroup;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

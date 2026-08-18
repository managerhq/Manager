using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerServer.Model;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade413(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.CustomReport>())
            {
                if (e.Select != null)
                {
                    foreach (var e2 in e.Select)
                    {
                        if (e2.SelectPrimaryField?.Key != null) e2.SelectPrimaryField.Key = e2.SelectPrimaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                        if (e2.SelectSecondaryField?.Key != null) e2.SelectSecondaryField.Key = e2.SelectSecondaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                    }
                }
                if (e.Where != null)
                {
                    foreach (var e2 in e.Where)
                    {
                        if (e2.WherePrimaryField?.Key != null) e2.WherePrimaryField.Key = e2.WherePrimaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                        if (e2.WhereSecondaryField?.Key != null) e2.WhereSecondaryField.Key = e2.WhereSecondaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                    }
                }
                if (e.OrderBy != null)
                {
                    foreach (var e2 in e.OrderBy)
                    {
                        if (e2.OrderByPrimaryField?.Key != null) e2.OrderByPrimaryField.Key = e2.OrderByPrimaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                        if (e2.OrderBySecondaryField?.Key != null) e2.OrderBySecondaryField.Key = e2.OrderBySecondaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                    }
                }
                if (e.GroupBy != null)
                {
                    foreach (var e2 in e.GroupBy)
                    {
                        if (e2.GroupByPrimaryField?.Key != null) e2.GroupByPrimaryField.Key = e2.GroupByPrimaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                        if (e2.GroupBySecondaryField?.Key != null) e2.GroupBySecondaryField.Key = e2.GroupBySecondaryField.Key.Replace("Manager.Query.", "ManagerServer.Query.").Replace("Manager.Model.", "ManagerServer.Model.");
                    }
                }

                list.Add(e);
            }
            return list;
        }
    }
}

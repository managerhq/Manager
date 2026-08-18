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
        private static async Task<IEnumerable<Model.Object>> Upgrade336(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.Obsolete_DueDate == DueDateType2.By && x.DueDateDate.HasValue))
            {
                e.DueDate = DueDateType.By;
                list.Add(e);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.Obsolete_DueDate == DueDateType2.By && x.DueDateDate.HasValue))
            {
                e.DueDate = DueDateType.By;
                list.Add(e);
            }
            return list;
        }
    }
}

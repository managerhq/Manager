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
        private static async Task<IEnumerable<Model.Object>> Upgrade48(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete07.TaxCode07>().ToArray())
            {
                list.Add(new Model.TaxCode() { Key = e.Key, Name = e.Code, Obsolete_Notes = e.Notes, Components = new[] { new Model.TaxCode.Component() { Name = e.Code, ComponentRate = e.Rate } } });
            }
            return list;
        }
    }
}

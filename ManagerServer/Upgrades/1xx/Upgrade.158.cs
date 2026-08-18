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
        private static async Task<IEnumerable<Model.Object>> Upgrade158(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var o = objects.OfType<ManagerServer.Model.Obsolete.Obsolete30.DateNumberFormat30>().SingleOrDefault(x => x.Key == new Guid("beee8855-c7e8-4568-8c10-9146972c2ce3"));
            if (o != null)
            {
                try
                {
                    var culture = new System.Globalization.CultureInfo(o.Culture);
                    list.Add(new ManagerServer.Model.Obsolete.Obsolete58.NumberFormat() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete58.NumberFormat)), DecimalSeparator = culture.NumberFormat.NumberDecimalSeparator, GroupSeparator = culture.NumberFormat.NumberGroupSeparator, GroupSizes = culture.NumberFormat.NumberGroupSizes });
                    if (o.ISO8601)
                    {
                        list.Add(new ManagerServer.Model.Obsolete.Obsolete58.DateFormat() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete58.DateFormat)), ShortDatePattern = "yyyy-MM-dd" });
                    }
                    else
                    {
                        list.Add(new ManagerServer.Model.Obsolete.Obsolete58.DateFormat() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete58.DateFormat)), ShortDatePattern = culture.DateTimeFormat.ShortDatePattern });
                    }
                }
                catch
                {
                }
            }
            return list;
        }
    }
}

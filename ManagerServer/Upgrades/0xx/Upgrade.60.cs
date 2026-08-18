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
        private static async Task<IEnumerable<Model.Object>> Upgrade60(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var generalSettings = objects.OfType<Model.Obsolete.Obsolete11.GeneralSettings11>().SingleOrDefault(x => x.Key == Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete11.GeneralSettings11))) ?? new Model.Obsolete.Obsolete11.GeneralSettings11() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete11.GeneralSettings11)) };
            generalSettings.TaxAudit = objects.OfType<Model.TaxAudit>().Any();
            generalSettings.TaxSummary = objects.OfType<Model.TaxSummary>().Any();
            list.Add(generalSettings);
            return list;
        }
    }
}

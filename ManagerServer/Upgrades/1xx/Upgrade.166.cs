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
        private static async Task<IEnumerable<Model.Object>> Upgrade166(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var pdfTemplate = objects.OfType<ManagerServer.Model.Obsolete.Obsolete32.PdfTemplate32>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete32.PdfTemplate32)));

            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete32.CustomTheme32>())
            {
                if (pdfTemplate != null && pdfTemplate.Key == e.Key) continue;

                list.Add(new ManagerServer.Model.CustomTheme() { Key = e.Key, Name = e.Name, Template = e.Definition });
            }
            return list.ToArray();
        }
    }
}

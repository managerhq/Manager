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
        private static async Task<IEnumerable<Model.Object>> Upgrade306(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var simpleBAS = objects.SingleOrDefault<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>(new Guid("6163552c-3d89-4a15-bf72-ef1df6c0a6ee"));
            var fullBAS = objects.SingleOrDefault<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>(new Guid("11acbfe1-0d24-4161-b366-fe905f2bcfd9"));
            if (simpleBAS != null)
            {
                list.Add(new ManagerServer.Model.Obsolete.ObsoleteSingleton() { Key = new Guid("6163552c-3d89-4a15-bf72-ef1df6c0a6ee") });
                if (fullBAS == null)
                {
                    simpleBAS.Key = new Guid("11acbfe1-0d24-4161-b366-fe905f2bcfd9");
                    list.Add(simpleBAS);
                }
                foreach (var e in objects.OfType<ManagerServer.Model.ReportTransformationReport>().Where(x => x.ReportTransformation == new Guid("6163552c-3d89-4a15-bf72-ef1df6c0a6ee")).ToArray())
                {
                    e.ReportTransformation = new Guid("11acbfe1-0d24-4161-b366-fe905f2bcfd9");
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

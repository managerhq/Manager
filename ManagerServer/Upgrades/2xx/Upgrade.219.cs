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
        private static async Task<IEnumerable<Model.Object>> Upgrade219(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var sources = new Dictionary<Guid, string>();

            sources.Add(new Guid("11acbfe10d244161b366fe905f2bcfd9"), "https://www.manager.io/localizations/au/11acbfe10d244161b366fe905f2bcfd9.json");
            sources.Add(new Guid("92b3815438fc479aa2962019f656d1e2"), "https://www.manager.io/localizations/au/92b3815438fc479aa2962019f656d1e2.json");
            sources.Add(new Guid("07332ba33e824dc194511350f5d84e24"), "https://www.manager.io/localizations/au/07332ba33e824dc194511350f5d84e24.json");
            sources.Add(new Guid("c4a0ccf791714e8eb39097f7052b1479"), "https://www.manager.io/localizations/au/c4a0ccf791714e8eb39097f7052b1479.json");
            sources.Add(new Guid("994cef796da34fa19998ad029a4358f0"), "https://www.manager.io/localizations/nz/994cef796da34fa19998ad029a4358f0.json");
            sources.Add(new Guid("b755a3ef32aa4eab89360e48b057f627"), "https://www.manager.io/localizations/nl/b755a3ef32aa4eab89360e48b057f627.json");
            sources.Add(new Guid("12e5e9fbd8e84fceaa338ba564117550"), "https://www.manager.io/localizations/gb/12e5e9fbd8e84fceaa338ba564117550.json");
            sources.Add(new Guid("734f9a89b04846c5b792e652057c381f"), "https://www.manager.io/localizations/sa/734f9a89b04846c5b792e652057c381f.json");

            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>().Where(x => sources.ContainsKey(x.Key)).ToArray())
            {
                e.Obsolete_Source = sources[e.Key];
                list.Add(e);
            }
            return list;
        }
    }
}

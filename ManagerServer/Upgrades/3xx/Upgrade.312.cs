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
        private static async Task<IEnumerable<Model.Object>> Upgrade312(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var businessDetails = objects.SingleOrDefault<ManagerServer.Model.BusinessDetails>(ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BusinessDetails)));
            if (businessDetails?.Obsolete_Country == "English|Saudi Arabia")
            {
                businessDetails.Obsolete_Country = "العربية|المملكة العربية السعودية";
            }

            return list;
        }
    }
}

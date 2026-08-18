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
        private static async Task<IEnumerable<Model.Object>> Upgrade315(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var emailSettings = objects.SingleOrDefault<ManagerServer.Model.EmailSettings>(ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.EmailSettings)));
            if (emailSettings != null && emailSettings.Obsolete_Format2 == EmailFormat.PDF)
            {
                list.Add(new ManagerServer.Model.InternalPdfGenerator()
                {
                    Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.InternalPdfGenerator)),
                    Enabled = true,
                    PageSize = emailSettings.Obsolete_PageSize
                });
            }
            return list;
        }
    }
}

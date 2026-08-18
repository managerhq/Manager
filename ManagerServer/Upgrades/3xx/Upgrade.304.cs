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
        private static async Task<IEnumerable<Model.Object>> Upgrade304(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var emailSettings = objects.SingleOrDefault<ManagerServer.Model.EmailSettings>(ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.EmailSettings)));
            if (emailSettings != null)
            {
                if (emailSettings.Obsolete_Format == 0) emailSettings.Obsolete_Format2 = EmailFormat.PDF;
                if (emailSettings.Obsolete_Format == 1) emailSettings.Obsolete_Format2 = EmailFormat.Link;
                if (emailSettings.Obsolete_Format == 2) emailSettings.Obsolete_Format2 = EmailFormat.PDF;
                list.Add(emailSettings);
            }
            return list;
        }
    }
}

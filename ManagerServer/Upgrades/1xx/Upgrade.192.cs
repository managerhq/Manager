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
        private static async Task<IEnumerable<Model.Object>> Upgrade192(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete88.User>())
            {
                if (!string.IsNullOrWhiteSpace(e.Obsolete_Session))
                {
                    var sessionKey = new Guid(Convert.FromBase64String(e.Obsolete_Session.Replace("_", "/").Replace("-", "+") + "=="));
                    e.Sessions = new List<Model.Obsolete.Obsolete88.Session>();
                    e.Sessions.Add(new Model.Obsolete.Obsolete88.Session() { Key = sessionKey });
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

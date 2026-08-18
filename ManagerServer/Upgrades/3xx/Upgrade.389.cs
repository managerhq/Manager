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
        private static async Task<IEnumerable<Model.Object>> Upgrade389(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            foreach (var e in objects.OfType<Model.Obsolete.Obsolete90.BankFeedProvider>())
            {
                if (e.Endpoint == "https://www.aussiebankfeeds.com")
                {
                    e.Endpoint = "https://basiq.manager.io";
                    e.Name = "Bank Feeds for Australian Financial Institutions (Powered by Basiq.io)";
                    list.Add(e);
                }
            }

            return list;
        }
    }
}

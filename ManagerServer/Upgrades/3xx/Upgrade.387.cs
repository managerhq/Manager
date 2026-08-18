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
        private static async Task<IEnumerable<Model.Object>> Upgrade387(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var businessDetails = objects.Single<BusinessDetails>();
            if (businessDetails.Obsolete_Country != "en-AU") return null;

            return [new Model.Obsolete.Obsolete90.BankFeedProvider()
            {
                Endpoint = "https://basiq.manager.io",
                Name = "Basiq.io for Australian & New Zealand Financial Instituitions"
            }];
        }
    }
}

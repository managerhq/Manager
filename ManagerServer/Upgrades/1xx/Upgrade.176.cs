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
        private static async Task<IEnumerable<Model.Object>> Upgrade176(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var activationKey = objects.OfType<ManagerServer.Model.Obsolete.Obsolete39.ActivationKey39>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete39.ActivationKey39)));
            if (activationKey == null) return null;

            if (activationKey.Obsolete_Distributor == new Guid("5d7bcc5b-d0da-4418-830d-cd07b53bd8ce")) activationKey.Code = 288707288;
            if (activationKey.Obsolete_Distributor == new Guid("3931af4e-c005-4ed0-b4c6-415bae09461b")) activationKey.Code = 959430981;

            return new[] { activationKey };
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade251(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount>())
            {
                var type = ManagerServer.Model.Object.GetTypeByGuid(e.Key);
                if (type != null)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize<ManagerServer.Model.Obsolete.Obsolete62.ProfitAndLossStatementBuiltInAccount>(ms, e);
                        ms.Position = 0;
                        var o = ProtoBuf.Serializer.NonGeneric.Deserialize(type, ms) as ManagerServer.Model.Object;
                        o.Key = e.Key;
                        list.Add(o);
                    }
                }
            }
            return list;
        }
    }
}

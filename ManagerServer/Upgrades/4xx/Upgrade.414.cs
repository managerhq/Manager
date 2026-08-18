using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerServer.Model;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade414(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<Model.CustomButton>())
            {
                if (e.Endpoint != null && e.Endpoint.Contains("www.luboshasko.com/extensions/sa/zatca-phase-1-qr-generator"))
                {
                    e.Endpoint = "zatcaextension.azurewebsites.net";
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

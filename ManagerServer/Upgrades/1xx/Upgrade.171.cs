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
        private static async Task<IEnumerable<Model.Object>> Upgrade171(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            return new ManagerServer.Model.Object[]
            {
                new ManagerServer.Model.Obsolete.Obsolete34.SalesInvoiceOverpaymentFix34() { Key = new Guid("889e3690-cd15-4bd4-92b0-82fc12f35abe") },
                new ManagerServer.Model.Obsolete.Obsolete34.PurchaseInvoiceOverpaymentFix34() { Key = new Guid("f6033be8-3bd6-468d-a396-05e4831cb418") }
            };
        }
    }
}

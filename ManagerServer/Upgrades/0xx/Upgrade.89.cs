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
        private static async Task<IEnumerable<Model.Object>> Upgrade89(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var o = objects.OfType<Model.Obsolete.Obsolete26.SalesInvoiceTemplate26>().SingleOrDefault(x => x.Key == new Guid("55c81ff0-2892-41fb-bff8-3fef6debba85"));
            if (o != null && o.Obsolete_AmountsIncludeTax) list.Add(new Model.Obsolete.Obsolete37.SalesInvoiceAmountsTaxInclusiveDefault37() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete37.SalesInvoiceAmountsTaxInclusiveDefault37)), Value = true });
            var o2 = objects.OfType<Model.Obsolete.Obsolete26.PurchaseOrderTemplate26>().SingleOrDefault(x => x.Key == new Guid("2f777546-9a69-44ec-90bf-56c38563b100"));
            if (o2 != null && o2.Obsolete_AmountsIncludeTax) list.Add(new Model.Obsolete.Obsolete37.PurchaseOrderAmountsTaxInclusiveDefault37() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete37.PurchaseOrderAmountsTaxInclusiveDefault37)), Value = true });
            return list;
        }
    }
}

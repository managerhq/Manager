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
        private static async Task<IEnumerable<Model.Object>> Upgrade198(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            list.Add(new ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment() { Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment)), AmountsIncludeTax = true, BankClearStatus = ManagerServer.Model.Enums.BankClearStatus.Cleared });
            return list;
        }
    }
}

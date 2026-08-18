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
        private static async Task<IEnumerable<Model.Object>> Upgrade197(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43>().Where(x => x.Key != ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete43.CashReceipt43))))
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44()
                {
                    Key = e.Key,
                    AmountsIncludeTax = e.AmountsIncludeTax,
                    CashAccount = e.CashAccount,
                    CustomFields = e.CustomFields,
                    CustomTheme = e.CustomTheme,
                    Date = e.Date,
                    Description = e.Description,
                    InventoryLocation = e.InventoryLocation,
                    Lines = e.Lines,
                    Contact = e.Payer,
                    Reference = e.Reference,
                    Theme = e.Theme,
                    Timestamp = e.Timestamp,
                    Type = Model.Obsolete.Obsolete44.CashTransactionType.Deposit
                });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete43.CashPayment43>().Where(x => x.Key != ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete43.CashPayment43))))
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44()
                {
                    Key = e.Key,
                    AmountsIncludeTax = e.AmountsIncludeTax,
                    CashAccount = e.CashAccount,
                    CustomFields = e.CustomFields,
                    CustomTheme = e.CustomTheme,
                    Date = e.Date,
                    Description = e.Description,
                    InventoryLocation = e.InventoryLocation,
                    Lines = e.Lines,
                    Contact = e.Payee,
                    Reference = e.Reference,
                    Theme = e.Theme,
                    Timestamp = e.Timestamp,
                    Type = Model.Obsolete.Obsolete44.CashTransactionType.Withdrawal
                });
            }
            return list;
        }
    }
}

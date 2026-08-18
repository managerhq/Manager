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
        private static async Task<IEnumerable<Model.Object>> Upgrade196(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42>().Where(x => x.Key != ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete42.BankReceipt42))))
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment()
                {
                    Key = e.Key,
                    AmountsIncludeTax = e.AmountsIncludeTax,
                    BankAccount = e.BankAccount,
                    BankClearDate = e.BankClearDate,
                    BankClearStatus = e.BankClearStatus,
                    CustomFields = e.CustomFields,
                    CustomTheme = e.CustomTheme,
                    Date = e.Date ?? DateTime.MinValue,
                    Description = e.Description,
                    InventoryLocation = e.InventoryLocation,
                    Lines = e.Lines,
                    Contact = e.Payer,
                    Obsolete_Reference = e.Reference,
                    Theme = e.Theme,
                    Timestamp = e.Timestamp,
                    Type = Model.Obsolete.Obsolete66.ReceiptOrPaymentType.Receipt
                });
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete42.BankPayment42>().Where(x => x.Key != ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete42.BankPayment42))))
            {
                list.Add(new ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment()
                {
                    Key = e.Key,
                    AmountsIncludeTax = e.AmountsIncludeTax,
                    BankAccount = e.BankAccount,
                    BankClearDate = e.BankClearDate,
                    BankClearStatus = e.BankClearStatus,
                    CustomFields = e.CustomFields,
                    CustomTheme = e.CustomTheme,
                    Date = e.Date ?? DateTime.MinValue,
                    Description = e.Description,
                    InventoryLocation = e.InventoryLocation,
                    Lines = e.Lines,
                    Contact = e.Payee,
                    Obsolete_Reference = e.Reference,
                    Theme = e.Theme,
                    Timestamp = e.Timestamp,
                    Type = Model.Obsolete.Obsolete66.ReceiptOrPaymentType.Payment,
                });
            }
            return list;
        }
    }
}

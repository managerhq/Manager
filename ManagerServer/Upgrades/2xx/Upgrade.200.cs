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
        private static async Task<IEnumerable<Model.Object>> Upgrade200(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44>().Where(x => x.Key != ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44))).ToArray())
            {
                var o = new ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment()
                {
                    Key = e.Key,
                    AmountsIncludeTax = e.AmountsIncludeTax,
                    BankAccount = e.CashAccount,
                    Contact = e.Contact,
                    CustomFields = e.CustomFields,
                    CustomTheme = e.CustomTheme,
                    Date = e.Date ?? DateTime.MinValue,
                    Description = e.Description,
                    InventoryLocation = e.InventoryLocation,
                    Lines = e.Lines,
                    Reference = e.Reference,
                    Theme = e.Theme,
                    Timestamp = e.Timestamp,
                    Obsolete_CopyFromCashTransaction = true
                };

                if (e.Type == Model.Obsolete.Obsolete44.CashTransactionType.Deposit) o.Type = Model.Obsolete.Obsolete66.ReceiptOrPaymentType.Receipt;
                if (e.Type == Model.Obsolete.Obsolete44.CashTransactionType.Withdrawal) o.Type = Model.Obsolete.Obsolete66.ReceiptOrPaymentType.Payment;

                list.Add(o);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete44.CashTransaction44))).ToArray())
            {
                e.Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment));
                list.Add(e);
            }
            return list;
        }
    }
}

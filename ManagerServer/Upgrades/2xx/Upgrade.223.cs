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
        private static async Task<IEnumerable<Model.Object>> Upgrade223(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.TaxCode>())
            {
                var dirty = false;
                if (!e.Account.HasValue)
                {
                    e.Account = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71");
                }
                if (e.Account.Value == new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71"))
                {
                    dirty = true;
                }
                if (e.Components != null)
                {
                    foreach (var e2 in e.Components)
                    {
                        if (!e2.ComponentAccount.HasValue)
                        {
                            e2.ComponentAccount = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71");
                        }
                        if (e2.ComponentAccount.Value == new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71"))
                        {
                            dirty = true;
                        }
                    }
                }

                if (dirty)
                {
                    list.Add(e);
                }
            }
            if (list.Any())
            {
                var taxPayable = new ManagerServer.Model.BalanceSheetAccount() { Key = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71"), Name = Strings.TaxPayable, Group = new Guid("ed5a19f6-12c5-45cc-b4b7-4e79f7ef50bc") };
                var taxPayableBuiltIn = objects.OfType<ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount>().SingleOrDefault(x => x.Key == taxPayable.Key);
                if (taxPayableBuiltIn != null)
                {
                    if (!string.IsNullOrWhiteSpace(taxPayableBuiltIn.Name)) taxPayable.Name = taxPayableBuiltIn.Name;
                    if (taxPayableBuiltIn.Obsolete_Code.HasValue) taxPayable.Code = taxPayableBuiltIn.Code;
                    if (taxPayableBuiltIn.Group.HasValue) taxPayable.Group = taxPayableBuiltIn.Group;
                    taxPayable.Obsolete_StartingBalance2 = taxPayableBuiltIn.Obsolete_StartingBalance2;
                    taxPayable.Obsolete_StartingBalanceType2 = taxPayableBuiltIn.Obsolete_StartingBalanceType;
                }

                list.Add(taxPayable);
            }
            return list;
        }
    }
}

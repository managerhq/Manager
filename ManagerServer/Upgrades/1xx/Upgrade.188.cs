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
        private static async Task<IEnumerable<Model.Object>> Upgrade188(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetAccount>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    e.Obsolete_DoNotReverse = true;
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalance2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete63.BalanceSheetBuiltInAccount>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalance2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Customer>().Where(x => !x.Obsolete_HasStartingBalance || x.Obsolete_StartingBalanceType == StartingBalanceType.PaidInAdvance))
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalance2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Supplier>().Where(x => !x.Obsolete_HasStartingBalance || x.Obsolete_StartingBalanceType == StartingBalanceType.AmountToPay))
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalance2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalance2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete78.CashAccount>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.StartingBalance != 0m)
                {
                    e.Obsolete_StartingBalance = e.StartingBalance;
                    e.StartingBalance = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.FixedAsset>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalanceAcquisitionCost2 != 0m || e.Obsolete_StartingBalanceAccumulatedDepreciation2 != 0m)
                {
                    e.Obsolete_StartingBalanceCost = e.Obsolete_StartingBalanceAcquisitionCost2;
                    e.Obsolete_StartingBalanceAccumulatedDepreciation = e.Obsolete_StartingBalanceAccumulatedDepreciation2;
                    e.Obsolete_StartingBalanceAcquisitionCost2 = 0m;
                    e.Obsolete_StartingBalanceAccumulatedDepreciation2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.IntangibleAsset>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalance2 != 0m || e.Obsolete_StartingBalanceAccumulatedAmortization2 != 0m)
                {
                    e.Obsolete_StartingBalanceCost = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalanceAccumulatedAmortization = e.Obsolete_StartingBalanceAccumulatedAmortization2;
                    e.Obsolete_StartingBalance2 = 0m;
                    e.Obsolete_StartingBalanceAccumulatedAmortization2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.CapitalAccount>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalanceAmount2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalanceAmount2;
                    e.Obsolete_StartingBalanceAmount2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.SpecialAccount>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalance2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalance2;
                    e.Obsolete_StartingBalance2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Employee>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalanceAmount2 != 0m)
                {
                    e.Obsolete_StartingBalance = e.Obsolete_StartingBalanceAmount2;
                    e.Obsolete_StartingBalanceAmount2 = 0m;
                    list.Add(e);
                }
            }
            foreach (var e in objects.OfType<ManagerServer.Model.InventoryItem>().Where(x => !x.Obsolete_HasStartingBalance))
            {
                if (e.Obsolete_StartingBalanceAverageCost2 != 0m || e.Obsolete_StartingBalanceQty2 != 0m)
                {
                    e.Obsolete_StartingBalanceCost = e.Obsolete_StartingBalanceAverageCost2;
                    e.Obsolete_StartingBalanceQty = e.Obsolete_StartingBalanceQty2;
                    e.Obsolete_StartingBalanceAverageCost2 = 0m;
                    e.Obsolete_StartingBalanceQty2 = 0m;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}

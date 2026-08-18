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
        private static async Task<IEnumerable<Model.Object>> Upgrade233(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.BalanceSheetAccount>().Where(x => x.Obsolete_ControlAccount).ToArray())
            {
                if (e.Obsolete_ControlAccountType == ControlAccountType.BankAccounts) list.Add(new Model.ControlAccountForBankAccounts() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.Customers) list.Add(new Model.ControlAccountForCustomers() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.Suppliers) list.Add(new Model.ControlAccountForSuppliers() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.InventoryItems) list.Add(new Model.ControlAccountForInventoryItems() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.FixedAssets) list.Add(new Model.ControlAccountForFixedAssets() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.IntangibleAssets) list.Add(new Model.ControlAccountForIntangibleAssets() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.CapitalAccounts) list.Add(new Model.ControlAccountForCapitalAccounts() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });
                if (e.Obsolete_ControlAccountType == ControlAccountType.SpecialAccounts) list.Add(new Model.ControlAccountForSpecialAccounts() { Key = e.Key, Code = e.Code, Group = e.Group, Name = e.Name, Position = e.Position });

            }
            return list;
        }
    }
}

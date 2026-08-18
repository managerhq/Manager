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
        private static async Task<IEnumerable<Model.Object>> Upgrade64(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var sampleChartOfAccounts = objects.OfType<Model.Obsolete.Obsolete10.SampleChartOfAccounts10>().SingleOrDefault(x => x.Key == Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete10.SampleChartOfAccounts10)));
            if (sampleChartOfAccounts != null)
            {
                if (sampleChartOfAccounts.AccountingFees) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("9377f131-0b33-4afc-8274-9205fba314e7"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Accounting fees" });
                if (sampleChartOfAccounts.AdvertisingAndPromotion) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("49dabb47-a650-4855-90fe-7ca20332fe59"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Advertising and promotion" });
                if (sampleChartOfAccounts.BankCharges) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("76cdf1b7-3454-4788-a0a1-478142cb05a5"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Bank charges" });
                if (sampleChartOfAccounts.ComputerEquipment) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("dffe0e41-7e82-4677-bf78-9e18605c37c8"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Computer equipment" });
                if (sampleChartOfAccounts.Donations) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("cdfb61a3-ab45-4c88-a839-6f6786ac78b0"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Donations" });
                if (sampleChartOfAccounts.Electricity) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("381bbb79-a54d-4547-8f92-7915debe58b5"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Electricity" });
                if (sampleChartOfAccounts.Entertainment) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("4d114e8e-2e48-4fe7-a420-ea356821566f"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Entertainment" });
                if (sampleChartOfAccounts.InterestReceived) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("58723f2e-1692-4383-b987-087de6c6f5e4"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income, Name = "Interest received" });
                if (sampleChartOfAccounts.LegalFees) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("7a8276be-1ba8-4e75-8d32-c82ded0099a8"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Legal fees" });
                if (sampleChartOfAccounts.MotorVehicleExpenses) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("4f7cd815-a6bf-41ef-a7f3-14dcd5421c54"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Motor vehicle expenses" });
                if (sampleChartOfAccounts.PrintingAndStationery) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("a7c4a5a1-32b6-4e0c-be00-23dd5108dffd"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Printing and stationery" });
                if (sampleChartOfAccounts.Rent) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("76755195-1537-40d0-b2f0-ee8bf25e584d"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Rent" });
                if (sampleChartOfAccounts.RepairsAndMaintenance) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("0ad475f7-68cf-4c09-978e-75383cf54263"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Repairs and maintenance" });
                if (sampleChartOfAccounts.Sales) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("94bf247c-d9b6-4254-8f64-16420cd9640a"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income, Name = "Sales" });
                if (sampleChartOfAccounts.Telephone) list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = new Guid("bf328f09-9887-4088-b64b-bf399af0c536"), Category = ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses, Name = "Telephone" });
            }
            return list;
        }
    }
}

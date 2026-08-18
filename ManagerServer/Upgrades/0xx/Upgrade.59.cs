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
        private static async Task<IEnumerable<Model.Object>> Upgrade59(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var plugins = objects.OfType<Model.Obsolete.Obsolete08.Plugin08>().ToDictionary(x => x.Key);
            if (plugins.ContainsKey(Model.Obsolete.Obsolete08.Plugins08.SampleChartOfAccounts))
            {
                list.Add(new Model.Obsolete.Obsolete10.SampleChartOfAccounts10()
                {
                    Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete10.SampleChartOfAccounts10)),
                    AccountingFees = true,
                    AdvertisingAndPromotion = true,
                    BankCharges = true,
                    Donations = true,
                    ComputerEquipment = true,
                    Electricity = true,
                    Entertainment = true,
                    InterestReceived = true,
                    LegalFees = true,
                    MotorVehicleExpenses = true,
                    PrintingAndStationery = true,
                    Rent = true,
                    RepairsAndMaintenance = true,
                    Sales = true,
                    Telephone = true
                });
            }
            return list;
        }
    }
}

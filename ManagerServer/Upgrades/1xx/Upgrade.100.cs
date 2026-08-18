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
        private static async Task<IEnumerable<Model.Object>> Upgrade100(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var earnings = objects.OfType<Model.PayslipEarningsItem>().ToDictionary(x => x.Name, x => x.Key);
            var deductions = objects.OfType<Model.PayslipDeductionItem>().ToDictionary(x => x.Name, x => x.Key);
            var contributions = objects.OfType<Model.PayslipContributionItem>().ToDictionary(x => x.Name, x => x.Key);

            foreach (var e in objects.OfType<Model.Payslip>().ToArray())
            {
                if (e.Earnings != null)
                {
                    foreach (var e2 in e.Earnings)
                    {
                        if (!string.IsNullOrWhiteSpace(e2.Obsolete_Description))
                        {
                            if (!earnings.ContainsKey(e2.Obsolete_Description))
                            {
                                var key = Guid.CreateVersion7();
                                earnings.Add(e2.Obsolete_Description, key);
                                list.Add(new Model.PayslipEarningsItem() { Key = key, Name = e2.Obsolete_Description });
                            }
                            e2.Item = earnings[e2.Obsolete_Description];
                        }
                    }
                }
                if (e.Deductions != null)
                {
                    foreach (var e2 in e.Deductions)
                    {
                        if (!string.IsNullOrWhiteSpace(e2.Obsolete_Description))
                        {
                            if (!deductions.ContainsKey(e2.Obsolete_Description))
                            {
                                var key = Guid.CreateVersion7();
                                deductions.Add(e2.Obsolete_Description, key);
                                list.Add(new Model.PayslipDeductionItem() { Key = key, Name = e2.Obsolete_Description });
                            }
                            e2.Item = deductions[e2.Obsolete_Description];
                        }
                    }
                }
                if (e.Contributions != null)
                {
                    foreach (var e2 in e.Contributions)
                    {
                        if (!string.IsNullOrWhiteSpace(e2.Obsolete_Description))
                        {
                            if (!contributions.ContainsKey(e2.Obsolete_Description))
                            {
                                var key = Guid.CreateVersion7();
                                contributions.Add(e2.Obsolete_Description, key);
                                list.Add(new Model.PayslipContributionItem() { Key = key, Name = e2.Obsolete_Description });
                            }
                            e2.Item = contributions[e2.Obsolete_Description];
                        }
                    }
                }
                list.Add(e);
            }
            return list;
        }
    }
}

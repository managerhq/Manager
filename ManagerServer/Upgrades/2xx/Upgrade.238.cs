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
        private static async Task<IEnumerable<Model.Object>> Upgrade238(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>().ToArray())
            {
                if (e.Obsolete_Type == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.TaxSummary)))
                {
                    foreach (var e2 in objects.OfType<ManagerServer.Model.TaxSummary>().ToArray())
                    {
                        list.Add(new ReportTransformationReport() { Key = Guid.CreateVersion7(), ReportTransformation = e.Key, AccountingMethod = e2.AccountingMethod, FromDate = e2.FromDate, ToDate = e2.ToDate, Description = e2.Description });
                    }
                }
                else if (e.Obsolete_Type == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.EmployeeSummary)))
                {
                    foreach (var e2 in objects.OfType<ManagerServer.Model.EmployeeSummary>().ToArray())
                    {
                        list.Add(new ReportTransformationReport() { Key = Guid.CreateVersion7(), ReportTransformation = e.Key, FromDate = e2.FromDate, ToDate = e2.ToDate, Employee = e2.Employee });
                    }
                }
                else if (e.Obsolete_Type == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.TaxablePurchasesPerSupplier)))
                {
                    foreach (var e2 in objects.OfType<ManagerServer.Model.TaxablePurchasesPerSupplier>().ToArray())
                    {
                        list.Add(new ReportTransformationReport() { Key = Guid.CreateVersion7(), ReportTransformation = e.Key, AccountingMethod = e2.AccountingMethod, FromDate = e2.FromDate, ToDate = e2.ToDate, Description = e2.Description });
                    }
                }
                else if (e.Obsolete_Type == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PayslipTotalsPerItemAndEmployee)))
                {
                    foreach (var e2 in objects.OfType<ManagerServer.Model.PayslipTotalsPerItemAndEmployee>().ToArray())
                    {
                        if (e2.Periods == null || e2.Periods.Length == 0 || e2.Periods[0] == null) continue;
                        list.Add(new ReportTransformationReport() { Key = Guid.CreateVersion7(), ReportTransformation = e.Key, FromDate = e2.Periods[0].FromDate, ToDate = e2.Periods[0].ToDate, Description = e2.Description });
                    }
                }
            }
            return list;
        }
    }
}

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
        private static async Task<IEnumerable<Model.Object>> Upgrade303(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var startDate = objects.SingleOrDefault<ManagerServer.Model.Obsolete.Obsolete72.StartDate>(ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete72.StartDate)));
            if (startDate != null && startDate.Date.HasValue)
            {
                var start = startDate.Date.Value;
                var retainedEarnings = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetRetainedEarningsAccount));

                foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.IssueDate < start && x.Lines != null))
                {
                    foreach (var e2 in e.Lines)
                    {
                        var account = objects.SingleOrDefault(e2.Account);
                        if (account is ISalesInvoiceAccount)
                        {
                            // All good
                        }
                        else
                        {
                            e2.Account = retainedEarnings;
                            list.Add(e);
                        }
                    }
                }

                foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.IssueDate < start && x.Lines != null))
                {
                    foreach (var e2 in e.Lines)
                    {
                        var account = objects.SingleOrDefault(e2.Account);
                        if (account is IPurchaseInvoiceAccount)
                        {
                            // All good
                        }
                        else
                        {
                            e2.Account = retainedEarnings;
                            list.Add(e);
                        }
                    }
                }
            }
            return list.Distinct();
        }
    }
}

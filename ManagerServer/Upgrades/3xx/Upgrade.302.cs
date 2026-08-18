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
        private static async Task<IEnumerable<Model.Object>> Upgrade302(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var startDate = objects.SingleOrDefault<ManagerServer.Model.Obsolete.Obsolete72.StartDate>(ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete72.StartDate)));
            if (startDate != null && startDate.Date.HasValue)
            {
                var start = startDate.Date.Value;
                foreach (var e in objects.OfType<ManagerServer.Model.AmortizationEntry>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, AmortizationEntry = e });
                foreach (var e in objects.OfType<ManagerServer.Model.BillableTime>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, BillableTime = e });
                foreach (var e in objects.OfType<ManagerServer.Model.CreditNote>().Where(x => x.IssueDate < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, CreditNote = e });
                foreach (var e in objects.OfType<ManagerServer.Model.DebitNote>().Where(x => x.IssueDate < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, DebitNote = e });
                foreach (var e in objects.OfType<ManagerServer.Model.DepreciationEntry>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, DepreciationEntry = e });
                foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, ExpenseClaim = e });
                foreach (var e in objects.OfType<ManagerServer.Model.InterAccountTransfer>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, InterAccountTransfer = e });
                foreach (var e in objects.OfType<ManagerServer.Model.InventoryTransfer>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, InventoryTransfer = e });
                foreach (var e in objects.OfType<ManagerServer.Model.InventoryWriteOff>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, InventoryWriteOff = e });
                foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, JournalEntry = e });
                foreach (var e in objects.OfType<ManagerServer.Model.LatePaymentFee>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, LatePaymentFee = e });
                foreach (var e in objects.OfType<ManagerServer.Model.Payslip>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, Payslip = e });
                // ProductionOrder for some reason was never ignored due to start date so we do not touch it here
                foreach (var e in objects.OfType<ManagerServer.Model.WithholdingTaxReceipt>().Where(x => x.Date < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, WithholdingTaxReceipt = e });

                // Receipts & Payments, just transaction cleared before start
                foreach (var e in objects.OfType<ManagerServer.Model.Payment>().Where(x => x.GetClearDate().HasValue && x.GetClearDate().Value < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, Payment = e });
                foreach (var e in objects.OfType<ManagerServer.Model.Receipt>().Where(x => x.GetClearDate().HasValue && x.GetClearDate().Value < start)) list.Add(new ManagerServer.Model.Obsolete.Obsolete72.Obsolete() { Key = e.Key, Receipt = e });

                // Fix partial payments
                foreach (var e in objects.OfType<ManagerServer.Model.SalesInvoice>().Where(x => x.IssueDate < start && x.Obsolete_PartialPayment && x.Obsolete_ConversionBalance != 0m))
                {
                    var lines = new List<ManagerServer.Model.SalesInvoice.Line>(e.Lines);
                    lines.Add(new SalesInvoice.Line()
                    {
                        Account = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetRetainedEarningsAccount)),
                        LineDescription = "Partial payment",
                        SalesUnitPrice = e.Obsolete_ConversionBalance * -1m
                    });
                    e.Lines = lines.ToArray();
                    list.Add(e);
                }
                foreach (var e in objects.OfType<ManagerServer.Model.PurchaseInvoice>().Where(x => x.IssueDate < start && x.Obsolete_PartialPayment && x.Obsolete_ConversionBalance != 0m))
                {
                    var lines = new List<ManagerServer.Model.PurchaseInvoice.Line>(e.Lines);
                    lines.Add(new PurchaseInvoice.Line()
                    {
                        Account = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetRetainedEarningsAccount)),
                        LineDescription = "Partial payment",
                        PurchaseUnitPrice = e.Obsolete_ConversionBalance * -1m
                    });
                    e.Lines = lines.ToArray();
                    list.Add(e);
                }
            }

            return list;
        }
    }
}

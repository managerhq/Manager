using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringReceipts
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringReceipt))]
    [Guide("Create receipts that repeat on a regular schedule.")]
    [Guide("Useful for regular customer payments like monthly subscriptions or rent income.")]
    [Fields(typeof(ManagerServer.Model.RecurringReceipt))]
    internal sealed class RecurringReceiptForm : NakedVueForm<ManagerServer.Model.RecurringReceipt>
    {
        protected override void OnSource(RecurringReceipt form, ManagerServer.Model.Object source)
        {
            if (source is Receipt receipt)
            {
                Copy(receipt, form);
            }
        }
    }
}

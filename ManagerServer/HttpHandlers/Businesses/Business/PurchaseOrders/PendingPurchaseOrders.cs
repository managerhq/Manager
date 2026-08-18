using System;
using ManagerServer.Model;
using System.Linq;
using ProtoBuf;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [Guid("321184E8-5946-4A82-8321-7CA829668FD7")]
    [Title(nameof(Strings.PurchaseOrders))]
    [Guide("The **Pending Purchase Orders** screen displays recurring purchase orders that are scheduled for automatic creation.")]
    [Guide("This screen helps you monitor which purchase orders will be generated based on their *recurring schedules*.")]
    [Guide("You can review upcoming orders before they are automatically created to ensure accuracy and proper timing.")]
    [Columns]
    internal sealed class PendingPurchaseOrders : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringPurchaseOrder>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("781FA1A8-3EBC-4D3D-BE2E-67103932151D")]
        [Guide("Shows the date when this recurring purchase order will be automatically created.")]
        public DateTime?[] GetNextIssueDate(RecurringPurchaseOrder[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("0A4FC7ED-0DFB-428F-93D0-A6B82DA948D4")]
        [Guide("Shows the *supplier* name and code for whom this purchase order will be created.")]
        public string[] GetSupplier(RecurringPurchaseOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("2EFF42AF-D2BD-41A6-9131-CB59DDA823E6")]
        [Guide("Shows the description from the recurring purchase order template.")]
        public string[] GetDescription(RecurringPurchaseOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}

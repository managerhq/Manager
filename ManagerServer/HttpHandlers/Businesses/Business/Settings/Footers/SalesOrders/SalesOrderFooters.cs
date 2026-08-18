using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesOrders))]
    [Title(nameof(Strings.SalesOrder))]
    [Guide("Footer templates allow you to add standard text sections at the bottom of your *sales orders*.")]
    [Guide("Create multiple footer templates to use for different types of sales orders, ensuring consistent communication with customers.")]
    [Columns]
    internal sealed class SalesOrderFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.SalesOrderFooter>
    {
        [Default]
        [Guide("Footers appear at the bottom of your *sales orders*, providing important order confirmation details and terms to customers.")]
        [Guide("Common footer content includes:")]
        [Guide("• Delivery schedules and shipping information")]
        [Guide("• Cancellation and return policies")]
        [Guide("• Warranty terms and conditions")]
        [Guide("• Order modification procedures")]
        [Guide("• Customer acknowledgment requirements")]
        [Header("Using Footer Templates")]
        [Guide("You can create multiple footer templates and select the appropriate one when creating each *sales order*.")]
        [Guide("This ensures consistent communication while allowing flexibility for different order types.")]
        [Guide("Enter a descriptive name for each footer template to easily identify its purpose, such as 'Rush Order Terms' or 'Custom Manufacturing Orders'.")]
        public string[] GetName(ManagerServer.Model.SalesOrderFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

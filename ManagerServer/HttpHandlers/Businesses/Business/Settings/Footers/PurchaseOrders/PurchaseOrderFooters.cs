using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseOrders))]
    [Title(nameof(Strings.PurchaseOrder))]
    [Guide("Footer templates allow you to add standardized text at the bottom of your purchase orders.")]
    [Guide("You can create multiple footer templates to use with different types of purchase orders or suppliers.")]
    [Header("Managing Footer Templates")]
    [Guide("Click the **New Footer** button to create a new footer template.")]
    [Guide("Each footer template can be customized with specific terms, conditions, and instructions relevant to your procurement needs.")]
    [Columns]
    internal sealed class PurchaseOrderFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.PurchaseOrderFooter>
    {
        [Default]
        [Guide("Footers appear at the bottom of your purchase orders and contain important information for your suppliers.")]
        [Header("Common Footer Content")]
        [Guide("Footer templates typically include terms and conditions such as:")]
        [Guide("• Shipping instructions and delivery requirements")]
        [Guide("• Quality inspection and acceptance procedures")]
        [Guide("• Return and rejection policies")]
        [Guide("• Payment terms and penalty clauses")]
        [Guide("• Compliance certifications and regulatory requirements")]
        [Header("Using Footer Templates")]
        [Guide("When creating a purchase order, you can select which footer template to use from a dropdown list.")]
        [Guide("This ensures consistent communication with suppliers while allowing flexibility for different types of purchases.")]
        [Guide("For example, you might have one footer for domestic orders and another for international shipments with customs information.")]
        [Header("Naming Your Templates")]
        [Guide("Give each footer template a clear, descriptive name that indicates its purpose.")]
        [Guide("Examples: 'Standard Terms', 'International Shipping', 'Urgent Orders', or 'Service Contracts'.")]
        [Guide("Good naming helps you quickly select the right footer when creating purchase orders.")]
        public string[] GetName(ManagerServer.Model.PurchaseOrderFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

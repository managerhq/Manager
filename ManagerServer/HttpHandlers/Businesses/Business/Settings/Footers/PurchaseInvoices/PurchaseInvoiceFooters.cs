using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseInvoices))]
    [Title(nameof(Strings.PurchaseInvoice))]
    [Guide("Purchase invoice footers allow you to add standardized text at the bottom of your purchase invoices.")]
    [Guide("You can create multiple footer templates and select the appropriate one when entering each purchase invoice.")]
    [Columns]
    internal sealed class PurchaseInvoiceFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.PurchaseInvoiceFooter>
    {
        [Default]
        [Guide("Footer templates help you maintain consistency across your purchase invoices by providing pre-written text sections.")]
        [Guide("Common uses for purchase invoice footers include:")]
        [Guide("• Internal approval requirements or authorization notes")]
        [Guide("• Purchase order references and cross-references")]
        [Guide("• Receiving instructions or warehouse delivery notes")]
        [Guide("• Compliance statements or regulatory requirements")]
        [Guide("• Payment terms specific to certain vendors")]
        [Header("Creating Footer Templates")]
        [Guide("Click the **New Footer** button to create a new footer template.")]
        [Guide("Give each footer template a descriptive name that clearly indicates its purpose, such as *Standard Vendor Terms*, *Capital Equipment Purchases*, or *International Suppliers*.")]
        [Guide("This makes it easy to select the appropriate footer when entering purchase invoices.")]
        public string[] GetName(ManagerServer.Model.PurchaseInvoiceFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesInvoices))]
    [Title(nameof(Strings.SalesInvoice))]
    [Guide("Sales invoice footers allow you to add customized text at the bottom of your sales invoices.")]
    [Guide("You can create multiple footer templates and assign them to different sales invoices based on your business needs.")]
    [Columns]
    internal sealed class SalesInvoiceFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.SalesInvoiceFooter>
    {
        [Default]
        [Guide("Footer templates help you maintain consistent messaging across your sales invoices while providing flexibility for different scenarios.")]
        [Header("Common Uses")]
        [Guide("Footers typically include *payment terms*, *bank account details*, *return policies*, *warranty information*, or *thank you messages*.")]
        [Guide("You can create specialized footers for different customer types, payment methods, or product categories.")]
        [Header("Setting Up Footer Templates")]
        [Guide("Click the **New Footer** button to create a new footer template.")]
        [Guide("Give each footer template a descriptive name to easily identify its purpose, such as 'Standard Payment Terms', 'International Customers', or '30-Day Terms'.")]
        [Guide("When creating a sales invoice, you can select the appropriate footer template from a dropdown list.")]
        public string[] GetName(ManagerServer.Model.SalesInvoiceFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

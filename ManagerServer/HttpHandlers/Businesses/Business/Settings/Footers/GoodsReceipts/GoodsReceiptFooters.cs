using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.GoodsReceipts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(GoodsReceipts))]
    [Title(nameof(Strings.GoodsReceipt))]
    [Guide("*Goods receipt footers* are customizable text sections that appear at the bottom of your goods receipts, providing important information about receiving procedures and documentation requirements.")]
    [Guide("You can create multiple footer templates to accommodate different types of deliveries, suppliers, or warehouse procedures.")]
    [Header("Purpose")]
    [Guide("Footers help standardize your receiving documentation by including consistent information on every goods receipt.")]
    [Guide("Each footer template can be tailored for specific scenarios, such as different product categories, supplier requirements, or warehouse locations.")]
    [Columns]
    internal sealed class GoodsReceiptFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.GoodsReceiptFooter>
    {
        [Default]
        [Guide("This table displays all your goods receipt footer templates. Each row represents a different footer that can be selected when creating goods receipts.")]
        [Header("Common Footer Content")]
        [Guide("Typical information included in goods receipt footers:")]
        [Guide("• **Inspection requirements** - Quality control procedures and acceptance criteria")]
        [Guide("• **Storage instructions** - Temperature requirements, handling precautions, or shelf life information")]
        [Guide("• **Discrepancy procedures** - Steps to follow when received quantities or conditions don't match the purchase order")]
        [Guide("• **Receiving notes** - Special instructions for warehouse staff or acknowledgment statements")]
        [Header("Managing Templates")]
        [Guide("To create a new footer template, click the **New Footer** button below the table.")]
        [Guide("Give each footer a descriptive name that clearly indicates its purpose, such as 'Perishable Goods Receipt', 'Equipment Delivery', or 'Chemical Materials Handling'.")]
        [Guide("When creating a goods receipt, you'll be able to select from your available footer templates, ensuring the appropriate information appears on each document.")]
        public string[] GetName(ManagerServer.Model.GoodsReceiptFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseOrder))]
    [Guide("The *Purchase Order* view displays comprehensive details about a specific purchase order, including supplier information, line items, quantities, prices, and totals.")]
    [Guide("This view provides a formatted presentation of the purchase order that matches how it will appear when printed or emailed to your supplier.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• **Edit** - Modify the purchase order details by clicking the **Edit** button")]
    [Guide("• **Print** - Generate a PDF version for printing or saving")]
    [Guide("• **Email** - Send the purchase order directly to the supplier's email address")]
    [Guide("• **Copy to** - Create new transactions based on this purchase order")]
    [Guide("The view automatically displays all relevant information including issue date, reference number, supplier details, line items with quantities and prices, applicable taxes, and the total amount.")]
    [LinkGuide("To learn about creating and editing purchase orders, see:", typeof(PurchaseOrderForm))]
    internal sealed class PurchaseOrderView : TransactionView<ManagerServer.Model.PurchaseOrder>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction), typeof(ManagerServer.Model.RecurringPurchaseOrder)];
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Supplier>(business.SingleOrDefault<PurchaseOrder>(Key)?.Supplier)?.Email;
        }

        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForPurchaseOrder>();
        }
    }
}
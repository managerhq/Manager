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

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.SalesOrder))]
    [Guide("The `Sales Order` view displays the complete details of a sales order document, providing a comprehensive overview of the order before it is converted to a sales invoice.")]
    [Guide("This view shows all essential order information including customer details, ordered items, quantities, unit prices, and totals. You can also see any applicable taxes, discounts, and the total amount due.")]
    [Guide("From this view, you can edit the sales order, email it to the customer, print it, or convert it to other transaction types such as a sales invoice when the order is ready to be fulfilled.")]
    [LinkGuide("For more information, see:", typeof(SalesOrderForm))]
    internal sealed class SalesOrderView : TransactionView<ManagerServer.Model.SalesOrder>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForSalesOrder>();
        }

        protected override string GetRecipient()
        {
            var business = ApplicationData.Businesses.Get(Business);
            return business.SingleOrDefault<Customer>(business.SingleOrDefault<SalesOrder>(Key)?.Customer)?.Email;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction),typeof(ManagerServer.Model.RecurringSalesOrder)];
        }
    }
}
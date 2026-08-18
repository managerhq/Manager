using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.CustomerPortal.Invoices
{
    [ProtoContract]
    class CustomerPortalInvoice : View<ManagerServer.Api.Businesses.Business.SalesInvoices.GetSalesInvoiceView>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.CustomerPortal.Quotes
{
    [ProtoContract]
    class CustomerPortalQuote : View<ManagerServer.Api.Businesses.Business.SalesQuotes.GetSalesQuoteView>
    {
    }
}

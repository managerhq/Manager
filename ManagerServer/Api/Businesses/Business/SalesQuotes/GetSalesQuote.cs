using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    internal sealed class GetSalesQuote : GetObjectEndpoint<Model.SalesQuote>
    {
    }
}

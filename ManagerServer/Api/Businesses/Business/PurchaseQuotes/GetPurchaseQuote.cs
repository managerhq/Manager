using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.PurchaseQuotes
{
    [ProtoContract]
    internal sealed class GetPurchaseQuote : GetObjectEndpoint<Model.PurchaseQuote>
    {
    }
}

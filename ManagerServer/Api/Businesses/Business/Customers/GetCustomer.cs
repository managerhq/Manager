using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Customers
{
    [ProtoContract]
    internal sealed class GetCustomer : GetObjectEndpoint<Model.Customer>
    {
    }
}

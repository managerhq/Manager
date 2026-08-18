using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Suppliers
{
    [ProtoContract]
    internal sealed class GetSupplier : GetObjectEndpoint<Model.Supplier>
    {
    }
}

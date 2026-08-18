using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    internal sealed class GetExpenseClaim : GetObjectEndpoint<Model.ExpenseClaim>
    {
    }
}

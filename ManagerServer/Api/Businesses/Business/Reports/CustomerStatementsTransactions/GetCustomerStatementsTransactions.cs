using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsTransactions
{
    [ProtoContract]
    internal sealed class GetCustomerStatementsTransactions : GetObjectEndpoint<Model.CustomerStatementsTransactions>
    {
    }
}

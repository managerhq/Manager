using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsTransactions
{
    [ProtoContract]
    internal sealed class GetCustomerStatementsTransactionsBatch : GetObjectBatchEndpoint<Model.CustomerStatementsTransactions, GetCustomerStatementsTransactions, PostCustomerStatementsTransactions, PutCustomerStatementsTransactions, DeleteCustomerStatementsTransactions>
    {
    }
}

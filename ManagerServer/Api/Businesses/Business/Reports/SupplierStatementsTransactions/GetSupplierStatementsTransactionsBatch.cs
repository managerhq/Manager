using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsTransactions
{
    [ProtoContract]
    internal sealed class GetSupplierStatementsTransactionsBatch : GetObjectBatchEndpoint<Model.SupplierStatementsTransactions, GetSupplierStatementsTransactions, PostSupplierStatementsTransactions, PutSupplierStatementsTransactions, DeleteSupplierStatementsTransactions>
    {
    }
}

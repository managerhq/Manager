using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    internal sealed class GetBalanceSheetBatch : GetObjectBatchEndpoint<Model.BalanceSheet, GetBalanceSheet, PostBalanceSheet, PutBalanceSheet, DeleteBalanceSheet>
    {
    }
}

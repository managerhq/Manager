using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee
{
    [ProtoContract]
    internal sealed class GetPayslipTotalsPerItemAndEmployeeBatch : GetObjectBatchEndpoint<Model.PayslipTotalsPerItemAndEmployee, GetPayslipTotalsPerItemAndEmployee, PostPayslipTotalsPerItemAndEmployee, PutPayslipTotalsPerItemAndEmployee, DeletePayslipTotalsPerItemAndEmployee>
    {
    }
}

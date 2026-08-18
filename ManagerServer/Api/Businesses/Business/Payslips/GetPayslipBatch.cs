using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Payslips
{
    [ProtoContract]
    internal sealed class GetPayslipBatch : GetObjectBatchEndpoint<Model.Payslip, GetPayslip, PostPayslip, PutPayslip, DeletePayslip>
    {
    }
}

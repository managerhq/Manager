using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Payslips
{
    [ProtoContract]
    internal sealed class GetPayslip : GetObjectEndpoint<Model.Payslip>
    {
    }
}

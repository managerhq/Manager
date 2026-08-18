using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    internal sealed class GetExpenseClaimsSummary : GetObjectEndpoint<Model.ExpenseClaimsSummary>
    {
    }
}

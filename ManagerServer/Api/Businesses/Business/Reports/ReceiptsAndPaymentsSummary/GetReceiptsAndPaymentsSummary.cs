using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    internal sealed class GetReceiptsAndPaymentsSummary : GetObjectEndpoint<Model.ReceiptsAndPaymentsSummary>
    {
    }
}

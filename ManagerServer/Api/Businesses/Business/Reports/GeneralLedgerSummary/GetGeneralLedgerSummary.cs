using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    internal sealed class GetGeneralLedgerSummary : GetObjectEndpoint<Model.GeneralLedgerSummary>
    {
    }
}

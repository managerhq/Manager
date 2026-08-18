using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerTransactions
{
    [ProtoContract]
    internal sealed class GetGeneralLedgerTransactions : GetObjectEndpoint<Model.GeneralLedgerTransactions>
    {
    }
}

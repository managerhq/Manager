using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    internal sealed class GetBankAccountSummary : GetObjectEndpoint<Model.BankAccountSummary>
    {
    }
}

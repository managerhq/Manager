using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    internal sealed class GetBankReconciliation : GetObjectEndpoint<Model.BankReconciliation>
    {
    }
}

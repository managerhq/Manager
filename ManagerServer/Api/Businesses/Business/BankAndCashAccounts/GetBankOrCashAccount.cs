using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    internal sealed class GetBankOrCashAccount : GetObjectEndpoint<Model.BankOrCashAccount>
    {
    }
}

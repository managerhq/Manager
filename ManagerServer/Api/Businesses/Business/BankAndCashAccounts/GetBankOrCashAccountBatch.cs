using ManagerServer.Endpoints;
using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    [Description("Get list of bank or cash accounts")]
    internal class GetBankOrCashAccountBatch : GetObjectBatchEndpoint<Model.BankOrCashAccount, GetBankOrCashAccount, PostBankOrCashAccount, PutBankOrCashAccount, DeleteBankOrCashAccount>
    {
    }
}

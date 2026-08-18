using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Payslips
{
    [ProtoContract]
    internal class GetPayslipTransactionJournal : GetTransactionJournalViewEndpoint<Payslip>
    {
    }
}

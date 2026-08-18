using ManagerServer.Model;
using System.Linq;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomReports
{
    [ProtoContract]
    [Title(nameof(Strings.CustomReport))]
    [Guide("The Custom Report form allows creating customized reports from your data.")]
    [Guide("Select fields, apply filters, and format the output to meet your specific reporting needs.")]
    [Fields(typeof(ManagerServer.Model.CustomReport))]
    internal sealed class CustomReportForm : NakedVueForm<ManagerServer.Model.CustomReport>
    {
        protected override void OnSource(CustomReport form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue && form.Select == null)
            {
                var type = typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction);
                //form.Data = new MemberInfo(typeof(Manager.Model.CustomReport.DataItem).GetMember(nameof(Manager.Model.CustomReport.DataItem.GeneralLedgerTransactions)).FirstOrDefault());
                form.Select = new CustomReport.SelectElement[]
                {
                    new CustomReport.SelectElement() { SelectPrimaryField = new MemberInfo(type.GetMember(nameof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.Date)).FirstOrDefault()) },
                    new CustomReport.SelectElement() { SelectPrimaryField = new MemberInfo(type.GetMember(nameof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.GeneralLedgerAccount)).FirstOrDefault()) },
                    new CustomReport.SelectElement() { SelectPrimaryField = new MemberInfo(type.GetMember(nameof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.Debit)).FirstOrDefault()) },
                    new CustomReport.SelectElement() { SelectPrimaryField = new MemberInfo(type.GetMember(nameof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.Credit)).FirstOrDefault()) },
                };
            }
        }
    }
}
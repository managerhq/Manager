using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BankAccountSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`BankAccountSummary` provides a comprehensive overview of bank account's financial activity over specific period of time.")]
    [Guide("To create a new `BankAccountSummary`, go to `Reports` tab, click `BankAccountSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.BankAccountSummary), name: nameof(Strings.NewReport))]
    internal sealed class BankAccountSummaryList : PersistentObjectTable<ManagerServer.Model.BankAccountSummary>
    {
        protected override ManagerServer.Model.BankAccountSummary[] Filter(ManagerServer.Model.BankAccountSummary[] rows)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            var allowed = userPermissions.GetBankCashAccounts();
            if (allowed.Length > 0)
            {
                return rows.Where(x => !x.BankAccount.HasValue || allowed.Contains(x.BankAccount.Value)).ToArray();
            }

            return rows;
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("6cdf07d7-edb3-4e7c-b644-ed887a8e6110")]
        public DateTime? GetFromDate(ManagerServer.Model.BankAccountSummary o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("0c2ba475-f1e0-4025-98e8-04246c924f3d")]
        public DateTime? GetToDate(ManagerServer.Model.BankAccountSummary o) => o.Periods?[0].ToDate;

        [Guid("38981eb6-c523-4296-98f2-10236dc745b8")]
        public BankOrCashAccount GetAccount(ManagerServer.Model.BankAccountSummary o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<BankOrCashAccount>(o.BankAccount);        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatement))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`ProfitAndLossStatement` provides a comprehensive overview of your company's financial performance, detailing revenues, expenses, and profits over a specific period to help you evaluate its profitability and operational efficiency.")]
    [Guide("To create a new `ProfitAndLossStatement`, go to `Reports` tab, click `ProfitAndLossStatement`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ProfitAndLossStatement), name: nameof(Strings.NewReport))]
    internal sealed class ProfitAndLossStatementList : PersistentObjectTable<ManagerServer.Model.ProfitAndLossStatement>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("5ebf03d5-c8bf-431b-9b68-435bba00a0be")]
        public DateTime? GetFromDate(ManagerServer.Model.ProfitAndLossStatement o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("9c253b07-b95c-4706-b5a6-67be1356aa4b")]
        public DateTime? GetToDate(ManagerServer.Model.ProfitAndLossStatement o) => o.Periods?[0].ToDate;

        [Guid("50ef745a-6551-4d1f-891a-ac5a2b385384")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.ProfitAndLossStatement o) => o.AccountingMethod;

        [Guid("9a64e452-8be5-4011-8ce5-6f4e27838181")]
        public string GetDescription(ManagerServer.Model.ProfitAndLossStatement o) => o.Description;
    }
}
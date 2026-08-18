using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The `CashFlowStatement` provides a comprehensive overview of your business's cash inflows and outflows, helping you monitor liquidity, assess financial stability.")]
    [Guide("To create a new `CashFlowStatement`, go to `Reports` tab, click `CashFlowStatement`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.CashFlowStatement), name: nameof(Strings.NewReport))]
    internal sealed class CashFlowStatementList : PersistentObjectTable<ManagerServer.Model.CashFlowStatement>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("c96298e8-7c78-469e-b706-5218ea11302e")]
        public DateTime? GetFromDate(ManagerServer.Model.CashFlowStatement o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("51b8bf5c-1798-4642-95ed-51d4eed938cb")]
        public DateTime? GetToDate(ManagerServer.Model.CashFlowStatement o) => o.Periods?[0].ToDate;

        [Guid("ce4b6690-788b-4f22-9add-0dfc9d0418b9")]
        public string GetDescription(ManagerServer.Model.CashFlowStatement o) => o.Description;
    }
}
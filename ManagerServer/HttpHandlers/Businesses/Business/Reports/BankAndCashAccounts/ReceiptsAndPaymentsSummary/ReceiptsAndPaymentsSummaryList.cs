using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ReceiptsAndPaymentsSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`ReceiptsAndPaymentsSummary` report provides a comprehensive overview of all cash inflows and outflows within a specified period, offering insights into your business's financial activity.")]
    [Guide("To create a new `ReceiptsAndPaymentsSummary`, go to `Reports` tab, click `ReceiptsAndPaymentsSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ReceiptsAndPaymentsSummary), name: nameof(Strings.NewReport))]
    internal sealed class ReceiptsAndPaymentsSummaryList : PersistentObjectTable<ManagerServer.Model.ReceiptsAndPaymentsSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b1275a34-dda0-4791-bd26-3b13ada7f99d")]
        public DateTime? GetFromDate(ManagerServer.Model.ReceiptsAndPaymentsSummary o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("599623d7-bf9c-45d4-878a-0c98d265d42f")]
        public DateTime? GetToDate(ManagerServer.Model.ReceiptsAndPaymentsSummary o) => o.Periods?[0].ToDate;

        [Guid("8f60c436-c877-4845-8836-b97acbee665d")]
        public string GetDescription(ManagerServer.Model.ReceiptsAndPaymentsSummary o) => o.Description;
    }
}
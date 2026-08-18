using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DivisionExceptionReport
{
    [ProtoContract]
    [Title(nameof(Strings.DivisionExceptionReport))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`DivisionExceptionReport` provides a overview of transactions which are not associated with any division. This is useful when you are running divisional accounting and every transaction should be associated with a division.")]
    [Guide("To create a new `DivisionExceptionReport`, go to `Reports` tab, click `DivisionExceptionReport`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.DivisionExceptionReport), name: nameof(Strings.NewReport))]
    internal sealed class DivisionExceptionReportList : PersistentObjectTable<ManagerServer.Model.DivisionExceptionReport>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("2b69d8b8-8d1f-44f7-9026-bbfcc57cab8b")]
        public DateTime? GetFromDate(ManagerServer.Model.DivisionExceptionReport o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("6244bcb4-8e51-4d7d-8a91-7174c9001478")]
        public DateTime? GetToDate(ManagerServer.Model.DivisionExceptionReport o) => o.ToDate;

        [Guid("9a007bb5-652c-4a44-83b2-92aead10c155")]
        public string GetDescription(ManagerServer.Model.DivisionExceptionReport o) => string.Empty;
    }
}
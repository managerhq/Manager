using ManagerServer.Api.Businesses.Business.Reports.ReceiptsAndPaymentsSummary;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ReceiptsAndPaymentsSummary))]
    [Guide("The Receipts & Payments Summary report shows cash movements by account.")]
    [Guide("It displays receipts and payments for the selected period with net cash changes.")]
    [LinkGuide("For more information see:", typeof(ReceiptsAndPaymentsSummaryForm))]
    internal sealed class ReceiptsAndPaymentsSummaryView : DefaultView<GetReceiptsAndPaymentsSummaryView>
    {
    }
}
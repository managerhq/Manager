using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ReceiptsAndPaymentsSummary), nameof(Strings.Edit))]
    [Guide("The Receipts & Payments Summary form is used to configure parameters for the report.")]
    [Guide("You can set date ranges and select which accounts to include in the summary.")]
    [Fields(typeof(ManagerServer.Model.ReceiptsAndPaymentsSummary))]
    internal sealed class ReceiptsAndPaymentsSummaryForm : NakedVueForm<ManagerServer.Model.ReceiptsAndPaymentsSummary>
    {
    }
}

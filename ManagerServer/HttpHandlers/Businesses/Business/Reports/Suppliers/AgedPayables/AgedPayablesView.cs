using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.AgedPayables;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedPayables), nameof(Strings.View))]
    [Guide("The **Aged Payables** report displays all outstanding amounts owed to suppliers, organized by how long the invoices have been unpaid.")]
    [Guide("This report helps you monitor your payment obligations by grouping unpaid invoices into aging periods (current, 30 days, 60 days, 90 days, and over 90 days).")]
    [Header("Understanding the Report")]
    [Guide("Each supplier with outstanding invoices appears as a row in the report, showing the total amount owed broken down by age.")]
    [Guide("The aging periods help identify which payments are overdue and by how long, allowing you to prioritize payments and manage cash flow effectively.")]
    [Guide("If a supplier has available credit from overpayments or credit notes, this reduces the total amount owed and is shown in the *Less: Credit* column.")]
    [Header("Report Features")]
    [Guide("You can expand supplier rows to see individual invoice details, including issue dates and reference numbers.")]
    [Guide("The report automatically converts foreign currency amounts to your base currency for accurate totals.")]
    [Guide("When multiple currencies are involved, suppliers are grouped by currency with subtotals for each currency group.")]
    [LinkGuide("To customize this report, see:", typeof(AgedPayablesForm))]
    internal sealed class AgedPayablesView : DefaultView<GetAgedPayablesView>
    {
    }
}

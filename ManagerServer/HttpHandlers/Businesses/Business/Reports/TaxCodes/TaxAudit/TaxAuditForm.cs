using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    [Title(nameof(Strings.TaxAudit))]
    [Guide("The *Tax Audit* report helps you verify that your tax calculations are accurate and compliant with tax regulations.")]
    [Guide("This report analyzes all transactions containing *tax codes* and shows detailed breakdowns of tax amounts, helping you identify any discrepancies or issues.")]
    [Guide("Use this report to prepare for tax audits, verify tax collection accuracy, and ensure proper tax reporting across all your transactions.")]
    [Fields(typeof(ManagerServer.Model.TaxAudit))]
    internal sealed class TaxAuditForm : NakedVueForm<ManagerServer.Model.TaxAudit>
    {
    }
}

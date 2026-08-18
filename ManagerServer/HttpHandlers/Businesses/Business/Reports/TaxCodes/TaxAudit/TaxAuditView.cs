using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxAudit;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    [Title(nameof(Strings.TaxAudit))]
    [Guide("The **Tax Audit** report helps you verify that tax codes have been correctly applied to transactions across all your accounts.")]
    [Guide("This report displays a comprehensive breakdown of transactions organized by general ledger account and tax code, making it easy to identify any inconsistencies or errors in tax code application.")]
    [Header("How to Use This Report")]
    [Guide("Use this report to review how tax codes are distributed across your accounts during a specific period. Each account shows the total amounts for transactions with no tax code as well as amounts for each tax code used.")]
    [Guide("Click on any amount in the report to drill down and view the individual transactions that make up that total. This allows you to investigate specific entries and verify their tax treatment.")]
    [Header("Accounting Methods")]
    [Guide("The report can be generated using either *accrual basis* or *cash basis* accounting methods. When using cash basis, only transactions that have been paid or received during the reporting period are included.")]
    [LinkGuide("To configure report parameters, see:", typeof(TaxAuditForm))]
    internal sealed class TaxAuditView : DefaultView<GetTaxAuditView>
    {
    }
}
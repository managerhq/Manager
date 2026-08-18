using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxReconciliation;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    [Title(nameof(Strings.TaxReconciliation))]
    [Guide("The **Tax Reconciliation** report provides a comprehensive view of your tax account movements and balances over specified periods.")]
    [Guide("This report helps you track how your tax liability changes through sales, purchases, payments, and other adjustments.")]
    [Header("Report Structure")]
    [Guide("The report is organized by tax account, showing for each account:")]
    [Guide("• *Opening balance* - Your tax liability at the start of each period")]
    [Guide("• *Payments* - Direct tax payments made to tax authorities")]
    [Guide("• *Receipts* - Tax refunds received from tax authorities")]
    [Guide("• *Other movements* - Journal entries and other adjustments affecting the tax account")]
    [Guide("• *Closing balance* - Your tax liability at the end of each period")]
    [Header("Tax Liability Movements")]
    [Guide("Below the closing balance, the report shows what created new tax liability during the period:")]
    [Guide("• *Tax on sales* - Tax collected from customers on your sales")]
    [Guide("• *Tax on purchases* - Tax paid to suppliers that can be claimed back")]
    [Guide("The total of these movements represents the net change in your tax liability for the period.")]
    [Header("Understanding the Numbers")]
    [Guide("Positive amounts increase your tax liability (you owe more tax), while negative amounts reduce it.")]
    [Guide("Click on any amount to view the underlying transactions that make up that figure.")]
    [Guide("The report can be generated using either *accrual basis* or *cash basis* accounting methods, depending on your tax requirements.")]
    [LinkGuide("To create or modify this report, see:", typeof(TaxReconciliationForm))]
    internal sealed class TaxReconciliationView : DefaultView<GetTaxReconciliationView>
    {
    }
}
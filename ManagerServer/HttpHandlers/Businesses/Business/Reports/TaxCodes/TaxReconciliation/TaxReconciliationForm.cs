using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    [Title(nameof(Strings.TaxReconciliation))]
    [Guide("The **Tax Reconciliation** report helps you verify that your recorded tax liabilities match your actual tax payments.")]
    [Guide("This report compares the tax amounts calculated on transactions with the tax payments made to tax authorities, helping you identify any discrepancies.")]
    [Guide("Use this report to ensure your tax records are accurate and complete before filing tax returns or during tax audits.")]
    [Fields(typeof(ManagerServer.Model.TaxReconciliation))]
    internal sealed class TaxReconciliationForm : NakedVueForm<ManagerServer.Model.TaxReconciliation>
    {
    }
}

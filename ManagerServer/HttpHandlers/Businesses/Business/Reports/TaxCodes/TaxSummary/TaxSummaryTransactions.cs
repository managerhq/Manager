using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    [Title(nameof(Strings.TaxSummary), nameof(Strings.Transactions))]
    [Guide("The **Tax Summary - Transactions** report provides a detailed view of all transactions that contribute to your tax summary figures.")]
    [Guide("This report breaks down tax transactions by *tax code*, showing individual line items from sales invoices, purchase invoices, receipts, payments, and other transactions that include tax components.")]
    [Guide("Use this report to verify the accuracy of your tax calculations and to identify specific transactions that make up your tax obligations or credits for any given period.")]
    [Guide("Each transaction displays the date, reference number, description, and the tax amount calculated based on the applicable *tax rate*.")]
    internal sealed class TaxSummaryTransactions : Summary.BaseGeneralLedgerAccountView<TaxSummaryTransactions>
    {
    }
}

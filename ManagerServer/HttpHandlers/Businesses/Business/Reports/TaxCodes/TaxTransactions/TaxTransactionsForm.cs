using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTransactions))]
    [Guide("The *Tax Transactions* report displays a detailed list of all transactions that contain tax codes within a specified date range.")]
    [Guide("This report helps you analyze tax-related activity by showing which transactions have contributed to your tax obligations or credits.")]
    [Guide("Each transaction is listed with its date, reference number, description, account, and the tax amount calculated based on the applied *tax code*.")]
    [Guide("Use this report to verify that tax has been correctly applied to transactions and to identify specific transactions for tax reporting purposes.")]
    [Fields(typeof(ManagerServer.Model.TaxTransactions))]
    internal sealed class TaxTransactionsForm : NakedVueForm<ManagerServer.Model.TaxTransactions>
    {
    }
}

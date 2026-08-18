using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierStatements), nameof(Strings.Transactions))]
    [Guide("Supplier statements with transactions provide a comprehensive record of all financial activities between your business and your suppliers. These statements show every transaction in chronological order, including purchases, payments, credit notes, and any other adjustments.")]
    [Guide("This report is essential for reconciling supplier accounts, verifying outstanding balances, and maintaining accurate records of your payables. Each transaction is listed with its date, reference number, description, and the resulting balance after each entry.")]
    [Guide("Use supplier transaction statements when you need to review the complete history with a supplier, resolve discrepancies, or provide documentation for audit purposes. The statements can be customized to show transactions for specific date ranges and can include opening balances to provide a complete picture of the account status.")]
    [Fields(typeof(ManagerServer.Model.SupplierStatementsTransactions))]
    internal sealed class SupplierStatementsTransactionsForm : NakedVueForm<ManagerServer.Model.SupplierStatementsTransactions>
    {
    }
}
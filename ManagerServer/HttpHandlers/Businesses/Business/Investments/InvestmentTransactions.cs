using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Investments
{
    [ProtoContract]
    [Title(nameof(Strings.Investments), nameof(Strings.Transactions))]
    [Guide("The Investment Transactions screen shows all transactions that affect the quantity of units you hold in a specific investment.")]
    [Guide("This comprehensive view helps you track how your investment holdings change over time through purchases, sales, and other adjustments.")]
    [Header("Transaction Information")]
    [Guide("Each transaction displays essential details including the date, description, quantity change, and your balance after the transaction.")]
    [Guide("Common transaction types include purchases (which increase your holdings), sales (which decrease them), and various adjustments that may affect your position.")]
    [Header("Reading the Transaction List")]
    [Guide("Transactions appear in chronological order, making it easy to follow the history of your investment.")]
    [Guide("The running balance column shows your total units held after each transaction, providing a clear picture of your position at any point in time.")]
    internal sealed class InvestmentTransactions : BaseGeneralLedgerTransactionsInheritable
    {
    }
}
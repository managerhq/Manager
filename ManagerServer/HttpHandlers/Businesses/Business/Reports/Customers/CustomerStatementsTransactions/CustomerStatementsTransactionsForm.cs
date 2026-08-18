using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerStatementsTransactions))]
    [Guide("The Customer Statements Transactions form is used to configure report parameters.")]
    [Guide("Select customers and date ranges to generate transaction-based statements.")]
    [Fields(typeof(ManagerServer.Model.CustomerStatementsTransactions))]
    internal sealed class CustomerStatementsTransactionsForm : NakedVueForm<ManagerServer.Model.CustomerStatementsTransactions>
    {
    }
}
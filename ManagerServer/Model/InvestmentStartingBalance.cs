using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;
using System.Collections.Generic;
using System.Linq;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6494cd10-ec22-4b6a-a34f-12c690a00098")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class InvestmentStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select the investment for which you want to enter a starting balance. This list shows all investments you have created under the `Investments` tab.")]
        [ProtoMember(1), Autocomplete(typeof(Investment))] public Guid? Investment { get; set; }
        [Guide("Enter the quantity of shares, units, or other investment units you already own. This represents your opening position in this investment when you begin using Manager.")]
        [ProtoMember(2), Prepend(nameof(Strings.Qty))] public decimal StartingBalance { get; set; }
        [Guide("Enter the market price per unit as of your starting date. Manager will automatically calculate the total market value by multiplying the quantity by this market price.")]
        [Guide("This establishes both your cost basis and initial market value for the investment.")]
        [ProtoMember(3), Prepend(nameof(Strings.MarketPrice)), NoLabel, AppendBaseCurrency] public decimal MarketPrice { get; set; }
        [Prepend(nameof(Strings.MarketValue)), NoLabel, Expression(Zero, Plus, nameof(StartingBalance), Times, nameof(MarketPrice), RoundToBaseCurrency), AppendBaseCurrency] public object MarketValue { get; set; }

        public override string GetReference()
        {
            return string.Empty;
        }

        public override string GetName()
        {
            return null;
        }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override string TransactionTitle => Strings.StartingBalance;

        public override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var transactions = new List<GeneralLedgerTransaction>();

            var baseCurrency = database.Single<BaseCurrency>();
            var investment = database.SingleOrDefault<Investment>(Investment);

            if (investment != null)
            {
                transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetInvestmentsAccount>(),
                        transactionAmount: baseCurrency.Round(StartingBalance * MarketPrice),
                        qty: StartingBalance,
                        transactionCurrency: baseCurrency,
                        transaction: this,
                        investment: investment
                    ));                
            }

            transactions.Add(new GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                        transactionAmount: transactions.Sum(x => x.TransactionAmount) * -1m,
                        transactionCurrency: baseCurrency,
                        transaction: this,
                        isBalancing: true
                    ));

            return transactions.ToArray();
        }
    }
}
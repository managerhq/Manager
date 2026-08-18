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
    [Guid("7995ce1d-d8b5-4034-8b87-fb0291a44102")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class IntangibleAssetStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select intangible asset that you have created under `IntangibleAssets`.")]
        [ProtoMember(1), Autocomplete(typeof(IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
        [Guide("Enter acquision cost of the intangible asset.")]
        [ProtoMember(2), Prepend(nameof(Strings.AcquisitionCost)), AppendBaseCurrency] public decimal StartingBalance { get; set; }
        [Guide("Enter accumulated amortization for the intangible asset.")]
        [ProtoMember(3), Prepend(nameof(Strings.AccumulatedAmortization)), NoLabel, AppendBaseCurrency] public decimal StartingBalanceAccumulatedAmortization { get; set; }
        [Prepend(nameof(Strings.BookValue)), NoLabel, Expression(Zero, Plus, nameof(StartingBalance), RoundToBaseCurrency, Minus, nameof(StartingBalanceAccumulatedAmortization), RoundToBaseCurrency), AppendBaseCurrency] public object BookValue { get; set; }

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
            var intangibleAsset = database.SingleOrDefault<IntangibleAsset>(IntangibleAsset);

            if (intangibleAsset != null)
            {
                transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAtCostAccount>(),
                        transactionAmount: StartingBalance,
                        transactionCurrency: baseCurrency,
                        transaction: this,
                        intangibleAsset: intangibleAsset
                    ));

                transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>(),
                        transactionAmount: StartingBalanceAccumulatedAmortization * -1m,
                        transactionCurrency: baseCurrency,
                        transaction: this,
                        intangibleAsset: intangibleAsset
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
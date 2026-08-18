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
    [Guid("f0bd4b36-7bd4-49c1-8d68-720704fa4309")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class FixedAssetStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select fixed asset that you have created under `FixedAssets`.")]
        [ProtoMember(1), Autocomplete(typeof(FixedAsset))] public Guid? FixedAsset { get; set; }
        [Guide("Enter acquision cost of the fixed asset.")]
        [ProtoMember(2), Prepend(nameof(Strings.AcquisitionCost)), AppendBaseCurrency] public decimal StartingBalance { get; set; }
        [Guide("Enter accumulated depreciation for the fixed asset.")]
        [ProtoMember(3), Prepend(nameof(Strings.AccumulatedDepreciation)), NoLabel, AppendBaseCurrency] public decimal StartingBalanceAccumulatedDepreciation { get; set; }
        [Prepend(nameof(Strings.BookValue)), NoLabel, Expression(Zero, Plus, nameof(StartingBalance), RoundToBaseCurrency, Minus, nameof(StartingBalanceAccumulatedDepreciation), RoundToBaseCurrency), AppendBaseCurrency] public object BookValue { get; set; }

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
            var fixedAsset = database.SingleOrDefault<FixedAsset>(FixedAsset);

            if (fixedAsset != null)
            {
                transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetFixedAssetsAtCostAccount>(),
                        transactionAmount: StartingBalance,
                        transactionCurrency: baseCurrency,
                        transaction: this,
                        fixedAsset: fixedAsset
                    ));

                transactions.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>(),
                        transactionAmount: StartingBalanceAccumulatedDepreciation * -1m,
                        transactionCurrency: baseCurrency,
                        transaction: this,
                        fixedAsset: fixedAsset
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
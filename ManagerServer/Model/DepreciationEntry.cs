using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [CustomFields]
    [Guid("75cdc055-6dec-4381-bc40-b670366e6abc")]
    public sealed class DepreciationEntry : Transaction, IHasAutomaticReference, IComparable<DepreciationEntry>, ICustomFields, ICode
    {
        [Guide("Enter the date for this depreciation entry. This determines which accounting period the depreciation expense is recorded in.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this depreciation entry. This helps identify and track depreciation calculations.")]
        [ProtoMember(4)] public string Reference { get; set; }
        [Guide("Optionally, add a description for this depreciation entry, such as 'Monthly depreciation' or 'Year-end depreciation adjustment'.")]
        [ProtoMember(2), Long, DoNotHide] public string Description { get; set; }
        [Guide("Enter the fixed assets to depreciate and their depreciation amounts. Each line represents a different asset being depreciated.")]
        [ProtoMember(3)] public Line[] Lines { get; set; }
        [ProtoMember(6)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(7)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(5), DoNotCopy] public bool AutomaticReference { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        [CustomFields]
        [ProtoContract]
        [Guid("300d0043-fd85-405a-aee5-eaf2da7bd7fa")]
        public sealed class Line : ITransactionLine
        {
            [Guide("Select the fixed asset to depreciate. Only active fixed assets with depreciation rates will appear.")]
            [ProtoMember(1), Autocomplete(typeof(FixedAsset))] public Guid? FixedAsset { get; set; }
            [Guide("Optionally assign this depreciation expense to a specific division for divisional reporting.")]
            [ProtoMember(3), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
            [ProtoMember(4)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(5)] public CustomFields CustomFields2 { get; set; }
            [Guide("Enter the depreciation amount for this asset. This reduces the asset's book value and creates a depreciation expense.")]
            [ProtoMember(2), Sum] public decimal Amount { get; set; }

            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
        }

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        public override string GetName()
        {
            return Reference;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (Lines == null) return [];

            var baseCurrency = database.Single<BaseCurrency>();
            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            foreach (var e in Lines)
            {
                if (!e.FixedAsset.HasValue) continue;
                var fixedAsset = database.SingleOrDefault<FixedAsset>(e.FixedAsset.Value);
                if (fixedAsset == null) continue;

                var creditAccount = database.Single<BalanceSheetFixedAssetsAccumulatedDepreciationAccount>() as IGeneralLedgerAccount;
                if (fixedAsset.DisposedFixedAsset && fixedAsset.DisposalDate.HasValue && fixedAsset.DisposalDate.Value < Date)
                {
                    creditAccount = database.Single<ProfitAndLossStatementAccountFixedAssetLossOnDisposal>();
                }

                var amount = baseCurrency.Round(e.Amount);
                var trackingCode = e.Division.HasValue ? database.SingleOrDefault<Division>(e.Division.Value) : null;

                IGeneralLedgerAccount expenseAccount = database.Single<ProfitAndLossStatementAccountFixedAssetDepreciation>();
                if (fixedAsset.CustomDepreciationExpenseAccount) expenseAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(fixedAsset.CustomDepreciationExpenseAccountSelection) ?? expenseAccount;

                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    transactionAmount: amount,
                    transactionCurrency: baseCurrency,
                    date: Date,
                    generalLedgerAccount: expenseAccount,
                    fixedAsset: fixedAsset,
                    trackingCode: trackingCode
                ));

                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    transactionAmount: amount * -1m,
                    transactionCurrency: baseCurrency,
                    date: Date,
                    generalLedgerAccount: creditAccount,
                    transactionLine: e,
                    fixedAsset: fixedAsset,
                    trackingCode: database.SingleOrDefault<Division>(fixedAsset.Division)
                ));
            }

            return list.ToArray();
        }

        int IComparable<DepreciationEntry>.CompareTo(DepreciationEntry other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }
    }
}

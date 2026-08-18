using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("94d5307e-4332-4545-ab1e-1528c9032b7d")]
    public sealed class IntangibleAsset : NamedObject, ICustomFields, IComparable<IntangibleAsset>, ICode
    {
        [Guide("Optionally, enter an intangible asset code or reference number. This helps identify and track individual intangible assets in your asset register.")]
        [ProtoMember(10), NoWrap, Short, Placeholder(nameof(Strings.Optional))] public string ItemCode { get; set; }
        [Guide("Enter the name of the intangible asset, such as 'Software License', 'Patent #123456', or 'Customer Database'.")]
        [ProtoMember(1)] public string ItemName { get; set; }
        [Guide("Enter the annual amortization rate as a percentage. For example, enter 20 for 20% amortization per year. This rate is used to calculate amortization expenses.")]
        [ProtoMember(15), Append("%")] public decimal AmortizationRate { get; set; }
        [Guide("Provide a detailed description of the intangible asset, including registration numbers, expiry dates, terms of use, or any other relevant information.")]
        [ProtoMember(2), Textarea, Long] public string Description { get; set; }
        [Guide("Assign this intangible asset to a specific division if you use divisional accounting. This helps track asset costs and amortization by division.")]
        [ProtoMember(20), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Select a control account if you want this intangible asset to use a different intangible assets account than the default. Useful for categorizing different types of intangible assets.")]
        [ProtoMember(12), Autocomplete(typeof(ControlAccountForIntangibleAssets))] public Guid? ControlAccountForIntangibleAssets { get; set; }
        [Guide("Select a control account for accumulated amortization if you want to use a different account than the default. This tracks the total amortization recorded for this asset.")]
        [ProtoMember(16), Autocomplete(typeof(ControlAccountForIntangibleAssetsAccumulatedAmortization))] public Guid? ControlAccountForIntangibleAssetsAccumulatedAmortization { get; set; }
        [Guide("Check this box if you want to use a custom amortization expense account instead of the default intangible assets amortization account.")]
        [ProtoMember(18)] public bool CustomAmortizationExpenseAccount { get; set; }
        [Guide("Select the profit and loss account where amortization expenses for this asset should be recorded.")]
        [ProtoMember(19), IfTrue(nameof(CustomAmortizationExpenseAccount)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountIntangibleAssetsAmortization))] public Guid? CustomAmortizationExpenseAccountSelection { get; set; }
        [Guide("Check this box when the intangible asset has been sold, written off, expired, or otherwise disposed of. This stops amortization calculations.")]
        [ProtoMember(6)] public bool DisposedIntangibleAsset { get; set; }
        [Guide("Enter the date when the intangible asset was disposed of. Amortization will be calculated up to this date.")]
        [ProtoMember(7), IfTrue(nameof(DisposedIntangibleAsset)), Prepend(nameof(Strings.DisposalDate)), NoLabel, NoWrap] public DateTime? DisposalDate { get; set; }
        [Guide("Select the profit and loss account where gains or losses on disposal should be recorded. The default is the intangible assets gains/loss on disposal account.")]
        [ProtoMember(17), IfTrue(nameof(DisposedIntangibleAsset)), Prepend(nameof(Strings.Account)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal))] public Guid? CustomExpenseAccountForDisposal { get; set; }
        [ProtoMember(9)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(21)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(4)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(8)] public decimal Obsolete_StartingBalanceAccumulatedAmortization2 { get; set; }
        [ProtoMember(22)] public DateTime Obsolete_StartingBalanceDate { get; set; }
        [ProtoMember(11)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(13)] public decimal Obsolete_StartingBalanceCost { get; set; }
        [ProtoMember(14)] public decimal Obsolete_StartingBalanceAccumulatedAmortization { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<IntangibleAsset>.CompareTo(IntangibleAsset other) => (IsInactive(), ItemCode, ItemName).CompareTo((other.IsInactive(), other.ItemCode, other.ItemName));
        string ICode.Code => ItemCode;

        public override bool IsInactive() => DisposedIntangibleAsset && DisposalDate.HasValue;

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ItemCode)) return ItemCode + " - " + ItemName;
                else return ItemName;
            }
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (DisposedIntangibleAsset && DisposalDate.HasValue) return false;
            if (filter is ControlAccountForIntangibleAssets && ControlAccountForIntangibleAssets != filter.Key) return false;
            return true;
        }

        public override string GetName()
        {
            return NameWithCode;
        }

        /*
        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        protected override Manager.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (StartingBalance == 0m && StartingBalanceAccumulatedAmortization == 0m) return null;

            var baseCurrency = database.Single<BaseCurrency>();

            var list = new List<Manager.Query.GeneralLedger.GeneralLedgerTransaction>();

            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                transaction: this,
                generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAtCostAccount>(),
                intangibleAsset: this,
                transactionAmount: StartingBalance,
                transactionCurrency: baseCurrency,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                transaction: this,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                intangibleAsset: this,
                transactionAmount: StartingBalance * -1,
                transactionCurrency: baseCurrency,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                transaction: this,
                generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>(),
                intangibleAsset: this,
                transactionAmount: StartingBalanceAccumulatedAmortization * -1,
                transactionCurrency: baseCurrency,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                transaction: this,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                intangibleAsset: this,
                transactionAmount: StartingBalanceAccumulatedAmortization,
                transactionCurrency: baseCurrency,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));

            return list.ToArray();
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }
        */
    }
}

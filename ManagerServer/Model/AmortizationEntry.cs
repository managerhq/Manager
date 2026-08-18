using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [CustomFields]
    [Guid("d33519a3-e8e0-4556-9833-b744a58dd2f7")]
    public sealed class AmortizationEntry : Transaction, IHasAutomaticReference, IComparable<AmortizationEntry>, ICustomFields
    {
        [Guide("The date when this amortization expense is recorded. This determines when the expense appears in your profit and loss statement.")]
        [Guide("Amortization spreads the cost of intangible assets over their useful life, similar to depreciation for tangible assets.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        
        [Guide("A unique reference number for this amortization entry. This can be automatically generated or manually entered for tracking purposes.")]
        [Guide("References help you locate specific amortization entries and maintain an audit trail of expense recognition.")]
        [ProtoMember(4)] public string Reference { get; set; }

        [Guide("Enter a description to explain this amortization entry. This helps identify the purpose of the amortization in reports and transaction lists.")]
        [Guide("Good descriptions might include the period covered, such as 'Monthly amortization for January 2024' or 'Q1 2024 software amortization'.")]
        [ProtoMember(2), Long, DoNotHide] public string Description { get; set; }

        [Guide("Specify individual amortization lines which have the following columns:")]
        [Guide("You can amortize multiple intangible assets in a single entry by adding multiple lines.")]
        [Fields(typeof(Line))]
        [ProtoMember(3)] public Line[] Lines { get; set; }
        [Guide("Custom field values for this amortization entry.")]
        [Guide("Use custom fields to track additional information specific to your business needs.")]
        [ProtoMember(6)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Custom field values for this amortization entry (newer format).")]
        [Guide("This supports the enhanced custom fields functionality with better data types and validation.")]
        [ProtoMember(7)] public CustomFields CustomFields2 { get; set; }
        [Guide("Check this box to automatically generate reference numbers.")]
        [Guide("Automatic numbering ensures unique references and saves time on data entry.")]
        [ProtoMember(5), DoNotCopy] public bool AutomaticReference { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        public override string GetReference() => Reference;

        [CustomFields]
        [ProtoContract]
        [Guid("b93f5bc7-99ae-4a4b-afc7-b7715bdfca45")]
        public sealed class Line : ITransactionLine
        {
            [Guide("Select the intangible asset being amortized. This list shows all intangible assets created under the `IntangibleAssets` tab.")]
            [Guide("Only active intangible assets with remaining book value appear in this list.")]
            [ProtoMember(1), Autocomplete(typeof(IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [Guide("Select the division for tracking purposes. This applies to the amortization expense.")]
            [Guide("Division tracking helps analyze costs by business segment or department.")]
            [ProtoMember(3), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
            [Guide("Custom field values for this line item.")]
            [ProtoMember(4)] public Dictionary<Guid, string> CustomFields { get; set; }
            [Guide("Custom field values for this line item (newer format).")]
            [ProtoMember(5)] public CustomFields CustomFields2 { get; set; }
            [Guide("Enter the amortization amount for this intangible asset. This represents the portion of the asset's cost being expensed in this period.")]
            [Guide("The amount should align with your amortization schedule and accounting policies for the asset.")]
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
                if (!e.IntangibleAsset.HasValue) continue;
                var intangibleAsset = database.SingleOrDefault<IntangibleAsset>(e.IntangibleAsset.Value);
                if (intangibleAsset == null) continue;
                if (intangibleAsset.DisposedIntangibleAsset && intangibleAsset.DisposalDate.HasValue && intangibleAsset.DisposalDate.Value < Date) continue;

                var amount = baseCurrency.Round(e.Amount);
                var trackingCode = e.Division.HasValue ? database.SingleOrDefault<Division>(e.Division.Value) : null;

                IGeneralLedgerAccount expenseAccount = database.Single<ProfitAndLossStatementAccountIntangibleAssetsAmortization>();
                if (intangibleAsset.CustomAmortizationExpenseAccount) expenseAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(intangibleAsset.CustomAmortizationExpenseAccountSelection) ?? expenseAccount;

                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    transactionAmount: amount,
                    transactionCurrency: baseCurrency,
                    date: Date,
                    generalLedgerAccount: expenseAccount,
                    intangibleAsset: intangibleAsset,
                    trackingCode: trackingCode
                ));

                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    transactionAmount: amount*-1m,
                    transactionCurrency: baseCurrency,
                    date: Date,
                    transactionLine: e,
                    generalLedgerAccount: database.Single<BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>(),
                    intangibleAsset: intangibleAsset,
                    trackingCode: database.SingleOrDefault<Division>(intangibleAsset.Division)
                ));
            }

            return list.ToArray();
        }

        int IComparable<AmortizationEntry>.CompareTo(AmortizationEntry other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }        
    }
}

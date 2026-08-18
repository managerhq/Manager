using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("7f368d97-8b7f-4b39-b156-dc66afd9496a")]
    public sealed class TaxCode : NamedObject, ICustomFields, IComparable<TaxCode>, ICode
    {
        [Guide("Enter a descriptive name for this tax code.")]
        [Guide("The name appears in dropdown lists when selecting tax codes on transactions.")]
        [Guide("Examples: 'VAT 20%', 'GST 10%', 'Sales Tax 8.5%', or 'Tax Exempt'.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }
        [Guide("Enter an optional label to display on customer-facing documents instead of the tax code name.")]
        [Guide("Use this when you want a simplified or localized tax description on invoices.")]
        [Guide("Example: Name 'VAT Standard 20%' could have label 'VAT' for cleaner invoices.")]
        [Guide("Leave blank to use the tax code name on all documents.")]
        [ProtoMember(21), NoWrap, Short, Placeholder(nameof(Strings.Optional))] public string Label { get; set; }
        [ProtoMember(18), NoWrap, Autocomplete(typeof(TaxCodeReportingCategory)), Short, Hidden] public Guid? ReportingCategory { get; set; }
        [ProtoMember(22), Autocomplete(typeof(TaxCodeReversedReportingCategory)), Short, Hidden] public Guid? ReportingCategoryReversed { get; set; }
        [Guide("Select the type of tax rate calculation for this tax code.")]
        [Guide("`ZeroRate` - For tax-exempt or zero-rated items. No tax calculation or posting required.")]
        [Guide("`TotalRate` - Posts 100% of the transaction amount as tax. Used for import duties or when the entire amount represents tax.")]
        [Guide("`CustomRate` - For standard tax calculations with specific percentage rates. Most common option.")]
        [ProtoMember(5), NoWrap] public TaxRate TaxRate { get; set; }
        [Guide("Choose how to structure your custom tax rate.")]
        [Guide("`SingleRate` - One tax percentage applied to the transaction amount. Most common for simple tax systems.")]
        [Guide("`MultipleRates` - Combine multiple tax components into one tax code. Useful for compound taxes like federal + state taxes.")]
        [Guide("Each component can have its own rate, account, and reporting category.")]
        [ProtoMember(6), NoWrap, IfEnum(nameof(TaxRate), (int)TaxRate.CustomRate)] public TaxRateType Type { get; set; }
        [ProtoMember(4), IfEnum(nameof(TaxRate), (int)TaxRate.CustomRate), IfEnum(nameof(Type), (int)TaxRateType.SingleRate), Append("%")] public decimal Rate { get; set; }
        [Guide("Select the balance sheet account where tax amounts will be posted.")]
        [Guide("The default `TaxPayable` account accumulates tax owed to authorities.")]
        [Guide("Create custom tax liability accounts in your `ChartOfAccounts` for specific tax types or jurisdictions.")]
        [Guide("This helps track different tax obligations separately and simplifies tax return preparation.")]
        [ProtoMember(7), NoWrap, IfNotZero(nameof(Rate)), IfEnumNot(nameof(TaxRate), (int)TaxRate.ZeroRate), IfEnumNot(nameof(Type), (int)TaxRateType.MultipleRates), Autocomplete(typeof(BalanceSheetAccount), Placeholder = typeof(BalanceSheetTaxPayableAccount))] public Guid? Account { get; set; }
        [ProtoMember(19), NoWrap, IfNotZero(nameof(Rate)), IfEnumNot(nameof(TaxRate), (int)TaxRate.ZeroRate), IfEnumNot(nameof(Type), (int)TaxRateType.MultipleRates), Label(nameof(Strings.ReportingCategory)), Autocomplete(typeof(TaxAmountReportingCategory)), Prepend(nameof(Strings.TaxAmount)), Hidden] public Guid? TaxAmountReportingCategory { get; set; }
        [ProtoMember(20), IfNotZero(nameof(Rate)), IfTrue(nameof(ReverseCharged)), IfEnumNot(nameof(TaxRate), (int)TaxRate.ZeroRate), IfEnumNot(nameof(Type), (int)TaxRateType.MultipleRates), Label(nameof(Strings.ReportingCategory)), Autocomplete(typeof(TaxAmountReversedReportingCategory)), Prepend(nameof(Strings.ReverseCharged)), Hidden] public Guid? TaxAmountReversedReportingCategory { get; set; }
        [ProtoMember(2), IfEnum(nameof(TaxRate), (int)TaxRate.CustomRate), IfEnum(nameof(Type), (int)TaxRateType.MultipleRates)] public Component[] Components { get; set; }
        [ProtoMember(11), IfEnum(nameof(TaxRate), (int)TaxRate.CustomRate)] public bool ReverseCharged { get; set; }
        [ProtoMember(13)] public bool CustomSalesInvoiceTitle { get; set; }
        [ProtoMember(14), IfTrue(nameof(CustomSalesInvoiceTitle)), Placeholder(nameof(Strings.Invoice)), NoLabel] public string SalesInvoiceTitle { get; set; }
        [ProtoMember(15)] public bool CustomCreditNoteTitle { get; set; }
        [ProtoMember(16), IfTrue(nameof(CustomCreditNoteTitle)), Placeholder(nameof(Strings.CreditNote)), NoLabel] public string CreditNoteTitle { get; set; }
        [Guide("Mark this tax code as inactive to hide it from dropdown selection lists.")]
        [Guide("Use this for tax codes that are no longer applicable due to rate changes or law updates.")]
        [Guide("Historical transactions using this tax code remain unchanged and appear correctly in reports.")]
        [Guide("Reactivate anytime by unchecking this box.")]
        [ProtoMember(10)] public bool Inactive { get; set; }
        [Guide("Add tax-specific information using `CustomFields`.")]
        [Guide("Track tax registration numbers, jurisdiction codes, or authority references.")]
        [Guide("Set up custom fields under `Settings` → `CustomFields` before using them here.")]
        [ProtoMember(17)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Use enhanced `CustomFields` for structured tax data.")]
        [Guide("Support for effective dates, tax ID numbers, and classification codes.")]
        [Guide("Configure field types and validation under `Settings` → `CustomFields`.")]
        [ProtoMember(23)] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Name;

        public override bool IsInactive()
        {
            return Inactive;
        }

        [ProtoContract]
        public sealed class Component
        {
            [Guide("Enter a descriptive name for this tax component.")]
            [Guide("Examples: 'Federal GST', 'Provincial PST', 'City Tax', or 'Environmental Levy'.")]
            [Guide("Component names appear separately on detailed tax reports.")]
            [ProtoMember(1), Short] public string Name { get; set; }
            [Guide("Enter the percentage rate for this tax component.")]
            [Guide("Rates are additive - a 5% federal + 7% state = 12% total tax.")]
            [Guide("Use negative rates for tax credits or reductions within a tax code.")]
            [ProtoMember(2), Append("%"), Label(nameof(Strings.Rate)), Sum] public decimal ComponentRate { get; set; }
            [Guide("Select the liability account for this specific tax component.")]
            [Guide("Using separate accounts for each component helps track obligations to different tax authorities.")]
            [Guide("Create component-specific accounts in your `ChartOfAccounts` first.")]
            [ProtoMember(4), IfNotZero(nameof(ComponentRate)), Autocomplete(typeof(BalanceSheetAccount), Placeholder = typeof(BalanceSheetTaxPayableAccount)), Label(nameof(Strings.Account))] public Guid? ComponentAccount { get; set; }
            [Guide("Assign a reporting category to group this component in tax reports.")]
            [Guide("Categories help separate different tax types for compliance reporting.")]
            [Guide("Configure tax reporting categories under `Settings` → `ReportingCategories`.")]
            [ProtoMember(5), IfNotZero(nameof(ComponentRate)), Autocomplete(typeof(TaxAmountReportingCategory)), Label(nameof(Strings.ReportingCategory)), Prepend(nameof(Strings.TaxAmount)), Hidden] public Guid? ComponentTaxAmountReportingCategory { get; set; }
            [Guide("Select the reporting category for reverse charge tax amounts.")]
            [Guide("Reverse charge applies when the buyer, not seller, must remit tax to authorities.")]
            [Guide("Common in B2B transactions, imports, and cross-border services.")]
            [ProtoMember(6), IfNotZero(nameof(ComponentRate)), IfTrue(nameof(ReverseCharged)), Autocomplete(typeof(TaxAmountReversedReportingCategory)), Label(nameof(Strings.ReportingCategory)), Prepend(nameof(Strings.ReverseCharged)), Hidden] public Guid? ComponentTaxAmountReversedReportingCategory { get; set; }

            [ProtoMember(3), Newtonsoft.Json.JsonIgnore] public bool Obsolete_IsCompound { get; set; }
        }

        [ProtoMember(3)] public string Obsolete_Notes { get; set; }
        [ProtoMember(8)] public bool Obsolete_FlatRate { get; set; }
        [ProtoMember(9)] public decimal Obsolete_FlatRateRate { get; set; }
        [ProtoMember(12)] public decimal Obsolete_ReverseChargedRate { get; set; }

        public sealed class TaxAmount
        {
            public Guid TaxCode { get; set; }
            public string Code { get; set; }
            public decimal Amount { get; set; }
            public Guid? Account { get; set; }
            public Guid? TaxReportingCategory { get; set; }
            public Guid? TaxReportingCategoryReversed { get; set; }
            public bool NegativeRate { get; set; }
        }

        public override string GetName()
        {
            return Name;
        }

        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(Label)) return Label;
            return GetName();
        }

        // This is needed for VueForm to calculate inline TaxAmount
        public decimal[] Rates
        {
            get
            {
                if (ReverseCharged) return new decimal[] { 0m };
                switch (TaxRate)
                {
                    case TaxRate.CustomRate:
                        if (Type == TaxRateType.SingleRate) return new decimal[] { Rate };
                        else return (Components ?? new Component[0]).Where(x => x.ComponentRate > 0m).Select(x => x.ComponentRate).ToArray();
                    default: return new decimal[] { 0m };
                }
            }
        }

        public bool HasDefaultControlAccount()
        {
            if (TaxRate == TaxRate.ZeroRate)
            {
                return false;
            }
            else if (TaxRate == TaxRate.CustomRate)
            {
                if (Type == TaxRateType.SingleRate)
                {
                    if (Rate == 0m || Account.HasValue) return false;
                }
                else if (Type == TaxRateType.MultipleRates)
                {
                    if (Components == null) return false;
                    else if (Components.All(x => x.ComponentRate == 0m || x.ComponentAccount.HasValue)) return false;
                }
            }
            else if (TaxRate == TaxRate.TotalRate)
            {
                if (Account.HasValue) return false;
            }
            return true;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public TaxAmount[] CalculateTaxAmounts(decimal amount, int numberOfDecimals, bool amountIncludeTax)
        {
            if (TaxRate != TaxRate.CustomRate) return [];

            if (Type == TaxRateType.SingleRate)
            {
                if (amountIncludeTax && !ReverseCharged)
                {
                    var taxAmount = Math.Round(amount / (100m + Rate) * Rate, numberOfDecimals, MidpointRounding.AwayFromZero);

                    return [ new TaxAmount() { TaxCode = Key, Code = GetDisplayName() ?? string.Empty, Amount = taxAmount, Account = Account, TaxReportingCategory = TaxAmountReportingCategory } ];
                }
                else
                {
                    var taxAmount = Math.Round(amount / 100m * Rate, numberOfDecimals, MidpointRounding.AwayFromZero);
                    return [ new TaxAmount() { TaxCode = Key, Code = GetDisplayName() ?? string.Empty, Amount = taxAmount, Account = Account, TaxReportingCategory = TaxAmountReportingCategory, TaxReportingCategoryReversed = TaxAmountReversedReportingCategory } ];
                }
            }
            else
            {
                if (Components != null)
                {
                    var taxRate = 0m;
                    foreach (var e in Components)
                    {
                        if (e.ComponentRate > 0m) taxRate += e.ComponentRate;
                    }

                    var result = new TaxAmount[Components.Length];
                    for (int i = 0; i < Components.Length; i++)
                    {
                        var component = Components[i];

                        if (amountIncludeTax && !ReverseCharged)
                        {
                            var taxAmount = Math.Round(amount / (100m.SafeAdd(taxRate)) * component.ComponentRate, numberOfDecimals, MidpointRounding.AwayFromZero);
                            result[i] = new TaxAmount() { TaxCode = Key, Code = component.Name ?? string.Empty, Amount = taxAmount, Account = component.ComponentAccount, TaxReportingCategory = component.ComponentTaxAmountReportingCategory, NegativeRate = component.ComponentRate < 0m };
                        }
                        else
                        {
                            var taxAmount = Math.Round(amount / 100m * component.ComponentRate, numberOfDecimals, MidpointRounding.AwayFromZero);
                            result[i] = new TaxAmount() { TaxCode = Key, Code = component.Name ?? string.Empty, Amount = taxAmount, Account = component.ComponentAccount, TaxReportingCategory = component.ComponentTaxAmountReportingCategory, NegativeRate = component.ComponentRate < 0m, TaxReportingCategoryReversed = component.ComponentTaxAmountReversedReportingCategory };
                        }
                    }
                    return result;
                }                
            }

            return [];
        }

        int IComparable<TaxCode>.CompareTo(TaxCode other)
        {
            return (Name, 0).CompareTo((other.Name, 0));
        }
    }
}

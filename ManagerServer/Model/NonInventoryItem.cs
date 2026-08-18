using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("7affe9ee-731f-4936-8acf-15cae7bcacee")]
    public sealed class NonInventoryItem : NamedObject, IItem, IPurchaseItem, ISaleItem, ICustomFields, IComparable<NonInventoryItem>, ICode
    {
        [Guide("Optionally, enter an item code. This helps identify the item quickly and can be used instead of the full name when creating transactions.")]
        [ProtoMember(1), NoWrap, Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Enter the name of the non-inventory item, such as 'Consulting Services', 'Shipping', or 'Installation Fee'.")]
        [ProtoMember(2), NoWrap] public string Name { get; set; }
        [Guide("Optionally, specify the unit of measurement, such as 'hour', 'each', 'service', or 'month'. This appears on invoices and other documents.")]
        [ProtoMember(15), Short, Placeholder(nameof(Strings.Optional)), Typeahead] public string UnitName { get; set; }
        [Guide("Select the income account to credit when this item is sold. This is typically a revenue or sales account.")]
        [ProtoMember(6), NoWrap, Prepend(nameof(Strings.Account)), Autocomplete(typeof(INonInventoryItemAccount))] public Guid? WhenSold { get; set; }
        [Guide("Select the expense or asset account to debit when this item is purchased. This could be an expense account or cost of goods sold.")]
        [ProtoMember(7), Prepend(nameof(Strings.Account)), Autocomplete(typeof(INonInventoryItemAccount))] public Guid? WhenPurchased { get; set; }
        [Guide("Check this box to set a default description that will automatically appear when this item is selected in transactions.")]
        [ProtoMember(17), Label(nameof(Strings.Autofill), nameof(Strings.LineDescription))] public bool HasDefaultLineDescription { get; set; }
        [Guide("Enter the default description text that will appear on invoices, quotes, and other documents when this item is selected.")]
        [ProtoMember(3), IfTrue(nameof(HasDefaultLineDescription)), NoLabel, Textarea] public string DefaultLineDescription { get; set; }
        [Guide("Check this box to set a default selling price that will automatically fill when creating sales transactions.")]
        [ProtoMember(18), Label(nameof(Strings.Autofill), nameof(Strings.Sales), nameof(Strings.UnitPrice))] public bool HasDefaultSalesUnitPrice { get; set; }
        [Guide("Enter the default selling price per unit. This can be overridden on individual transactions.")]
        [ProtoMember(4), IfTrue(nameof(HasDefaultSalesUnitPrice)), NoLabel] public decimal DefaultSalesUnitPrice { get; set; }
        [Guide("Check this box to set a default purchase price that will automatically fill when creating purchase transactions.")]
        [ProtoMember(19), Label(nameof(Strings.Autofill), nameof(Strings.Purchases), nameof(Strings.UnitPrice))] public bool HasDefaultPurchaseUnitPrice { get; set; }
        [Guide("Enter the default purchase price per unit. This can be overridden on individual transactions.")]
        [ProtoMember(5), IfTrue(nameof(HasDefaultPurchaseUnitPrice)), NoLabel] public decimal DefaultPurchaseUnitPrice { get; set; }
        [Guide("Check this box to set a default tax code that will automatically apply when this item is used in transactions.")]
        [ProtoMember(20), Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [Guide("Select the default tax code for this item. This determines the tax rate applied when the item is sold or purchased.")]
        [ProtoMember(8), IfTrue(nameof(HasDefaultTaxCode)), Short, NoLabel, Autocomplete(typeof(TaxCode))] public Guid? DefaultTaxCode { get; set; }
        [Guide("Check this box to assign a default division that will automatically apply when this item is used in transactions.")]
        [ProtoMember(22), Label(nameof(Strings.Autofill), nameof(Strings.Division))] public bool HasDefaultDivision { get; set; }
        [Guide("Select the default division for this item. This helps track income and expenses by division when this item is used.")]
        [ProtoMember(23), Autocomplete(typeof(Division)), IfTrue(nameof(HasDefaultDivision)), NoLabel, Short] public Guid? DefaultDivision { get; set; }
        [Guide("Check this box to show only the item code (not the name) on printed documents like invoices. Useful for internal codes you don't want customers to see.")]
        [ProtoMember(16)] public bool HideItemNameOnPrintedDocuments { get; set; }
        [Guide("Mark this item as inactive to hide it from dropdown lists while preserving historical transactions. Useful for discontinued items.")]
        [ProtoMember(12)] public bool Inactive { get; set; }
        [ProtoMember(10)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(21)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(9)] public Guid? Obsolete_PurchaseTaxCode { get; set; }
        [ProtoMember(11)] public Guid? Obsolete_Division { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        public bool HasCostOfGoodsSold => false;
        string ICode.Code => Code;

        public override bool IsInactive()
        {
            return Inactive;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
                else return Name;
            }
        }

        Guid IItem.Key => Key;
        public bool HasDefaultQty => false;
        public decimal? DefaultQty => null;
        public Guid? PurchaseItemAccount => WhenPurchased ?? ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetSuspenseAccount));
        public Guid? SaleItemAccount => WhenSold ?? ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetSuspenseAccount));

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public string GetNameWithCode()
        {
            return NameWithCode;
        }

        public string GetDisplayName()
        {
            if (HideItemNameOnPrintedDocuments) return Code;
            return (Code + " " + Name).Trim();
        }

        public override string GetName()
        {
            return NameWithCode;
        }

        public string GetCode()
        {
            return Code;
        }

        public string GetUnitName()
        {
            return UnitName;
        }

        int IComparable<NonInventoryItem>.CompareTo(NonInventoryItem other)
        {
            return (Inactive, Code, Name).CompareTo((other.Inactive, other.Code, other.Name));
        }
    }
}

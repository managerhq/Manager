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
    [CustomFields]
    [ProtoContract]
    [Guid("efc4f2cc-acf0-4815-a9a8-13bae00c6167")]
    public sealed class InventoryKit : NamedObject, IItem, ISaleItem, ICustomFields, IComparable<InventoryKit>, ICode
    {
        [Guide("Enter a code for this kit. This provides a short identifier for invoices and reports.")]
        [ProtoMember(6), Short, NoWrap, Placeholder(nameof(Strings.Optional))] public string ItemCode { get; set; }
        [Guide("Enter the name or description of this kit. This identifies what the bundle contains.")]
        [ProtoMember(1), NoWrap] public string ItemName { get; set; }
        [Guide("Enter the unit of measure for this kit, such as 'Set', 'Kit', or 'Bundle'.")]
        [ProtoMember(7), Short, Placeholder(nameof(Strings.Optional)), Typeahead] public string UnitName { get; set; }
        [Guide("Define the inventory items that make up this kit. Each component will be depleted when the kit is sold.")]
        [ProtoMember(3)] public Item[] BillOfMaterials { get; set; }
        [Guide("Assign this kit to a division for tracking divisional sales and inventory.")]
        [ProtoMember(12), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Check this box to set a default description that appears on sales documents when this kit is selected.")]
        [ProtoMember(14), Label(nameof(Strings.Autofill), nameof(Strings.LineDescription))] public bool HasDefaultLineDescription { get; set; }
        [Guide("Enter the default description to use on invoices and quotes. This can be overridden on individual transactions.")]
        [ProtoMember(2), IfTrue(nameof(HasDefaultLineDescription)), NoLabel, Textarea] public string DefaultLineDescription { get; set; }
        [Guide("Check this box to set a default selling price for this kit.")]
        [ProtoMember(15), Label(nameof(Strings.Autofill), nameof(Strings.UnitPrice))] public bool HasDefaultSalesUnitPrice { get; set; }
        [Guide("Enter the default selling price. This will automatically populate when the kit is selected on sales transactions.")]
        [ProtoMember(4), IfTrue(nameof(HasDefaultSalesUnitPrice)), NoLabel] public decimal DefaultSalesUnitPrice { get; set; }
        [Guide("Check this box to assign sales of this kit to a specific division by default.")]
        [ProtoMember(18), Label(nameof(Strings.Autofill), nameof(Strings.Sales), nameof(Strings.Division))] public bool HasDefaultDivision { get; set; }
        [Guide("Select the default division for sales transactions. This can be changed on individual transactions.")]
        [ProtoMember(19), Autocomplete(typeof(Division)), IfTrue(nameof(HasDefaultDivision)), NoLabel, Short] public Guid? DefaultDivision { get; set; }
        [Guide("Check this box to apply a default tax code when this kit is sold.")]
        [ProtoMember(16), Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [Guide("Select the default tax code for this kit. This determines the tax rate on sales.")]
        [ProtoMember(5), IfTrue(nameof(HasDefaultTaxCode)), Autocomplete(typeof(TaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [Guide("Check this box to use a custom income account instead of the default inventory sales account.")]
        [ProtoMember(10)] public bool CustomIncomeAccount { get; set; }
        [Guide("Select the income account where sales of this kit should be recorded.")]
        [ProtoMember(9), IfTrue(nameof(CustomIncomeAccount)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountInventorySales))] public Guid? IncomeAccount { get; set; }
        [Guide("Check this box to show only the item code on printed documents, hiding the item name.")]
        [ProtoMember(13)] public bool HideItemNameOnPrintedDocuments { get; set; }
        [Guide("Check this box to deactivate this kit. Inactive kits won't appear in selection lists but retain their history.")]
        [ProtoMember(8)] public bool Inactive { get; set; }
        [ProtoMember(11)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(17)] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        public bool HasCostOfGoodsSold => true;
        string ICode.Code => ItemCode;

        public override bool IsInactive()
        {
            return Inactive;
        }

        [ProtoContract]
        public sealed class Item
        {
            [Guide("Select an inventory item that is part of this kit. This item's quantity will be reduced when the kit is sold.")]
            [ProtoMember(1), Autocomplete(typeof(InventoryItem))] public Guid? InventoryItem { get; set; }
            [Guide("Enter the quantity of this item needed for one unit of the kit.")]
            [ProtoMember(3)] public decimal Qty { get; set; }

            [ProtoMember(2)] public decimal? Obsolete_Qty { get; set; }
        }

        public Guid GetIncomeAccount()
        {
            if (CustomIncomeAccount && IncomeAccount.HasValue) return IncomeAccount.Value;
            else return ManagerServer.Model.Master.AccountKeys.InventorySales;
        }

        public string GetDisplayName()
        {
            if (HideItemNameOnPrintedDocuments) return ItemCode;
            return (ItemCode + " " + ItemName).Trim();
        }

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
            if (Inactive) return false;
            return true;
        }

        Guid IItem.Key => Key;
        public bool HasDefaultQty => true;
        public decimal? DefaultQty => 1m;
        public Guid? SaleItemAccount => (CustomIncomeAccount ? IncomeAccount : null) ?? ManagerServer.Model.Object.GetGuidByType(typeof(ProfitAndLossStatementAccountInventorySales));

        public string GetNameWithCode()
        {
            return NameWithCode;
        }

        public override string GetName()
        {
            return NameWithCode;
        }

        public string GetCode()
        {
            return ItemCode;
        }

        public string GetUnitName()
        {
            return UnitName;
        }

        int IComparable<InventoryKit>.CompareTo(InventoryKit other)
        {
            return (Inactive, ItemCode, ItemName).CompareTo((other.Inactive, other.ItemCode, other.ItemName));
        }
    }
}

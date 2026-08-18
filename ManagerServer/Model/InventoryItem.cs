using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("0dbdbf8a-d80c-48e6-b453-bb7862445b7c")]    
    public sealed class InventoryItem : NamedObject, IItem, IPurchaseItem, ISaleItem, ICustomFields, IComparable<InventoryItem>, ICode
    {
        [Guide("Enter a unique code or SKU to identify this inventory item.")]
        [Guide("Item codes are optional but highly recommended for efficient inventory management. They appear on all transactions and reports.")]
        [Guide("Common formats include manufacturer part numbers, internal SKUs, or barcode numbers.")]
        [ProtoMember(1), Short, NoWrap, Placeholder(nameof(Strings.Optional))] public string ItemCode { get; set; }

        [Guide("Enter the full name or description of the inventory item.")]
        [Guide("This name appears on sales and purchase documents, so make it clear and descriptive for customers and suppliers.")]
        [Guide("Examples: 'Widget Model A-100', 'Blue Cotton T-Shirt Size L', or 'Consulting Services - 1 Hour'.")]
        [ProtoMember(11), NoWrap] public string ItemName { get; set; }

        [Guide("Enter the unit of measure for this inventory item, such as 'kg', 'box', 'hour', or 'meter'.")]
        [Guide("The unit name appears on all sales and purchase documents after quantities. For example, '5 boxes' instead of just '5'.")]
        [Guide("Leave blank if you sell items individually without specific units.")]
        [ProtoMember(13), Short, Placeholder(nameof(Strings.Optional)), Typeahead] public string UnitName { get; set; }

        [Guide("Select the inventory valuation method that determines how costs are calculated when items are sold.")]
        [Guide("`FIFO` (First In, First Out) - Oldest items are sold first, commonly used for perishable goods.")]
        [Guide("`Weighted Average` - Costs are averaged across all units, suitable for homogeneous products.")]
        [Guide("The valuation method affects your cost of goods sold and inventory value on financial reports.")]
        [ProtoMember(38)] public InventoryValuationMethod ValuationMethod { get; set; }

        [Guide("Assign this inventory item to a specific division for divisional profit reporting.")]
        [Guide("All sales and purchases of this item will be allocated to the selected division by default.")]
        [Guide("This field only appears if divisions are enabled under `Settings` → `Divisions`.")]
        [ProtoMember(17), Autocomplete(typeof(Division))] public Guid? Division { get; set; }

        [Guide("Select a custom control account if this item should use a different inventory account than the default.")]
        [Guide("Custom control accounts help categorize different types of inventory, such as raw materials vs finished goods, or by product line.")]
        [Guide("This field only appears if custom control accounts for inventory have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(20), Autocomplete(typeof(ControlAccountForInventoryItems))] public Guid? ControlAccount { get; set; }

        [Guide("Enable reorder point tracking to automatically calculate when to reorder this item.")]
        [Guide("Enter the desired quantity to maintain in stock. When inventory falls below this level, the `Qty to Order` column in the `Inventory Items` tab will show how much to order.")]
        [Guide("This helps prevent stockouts by alerting you when inventory levels are low.")]
        [ProtoMember(36)] public bool ReorderPoint { get; set; }
        [ProtoMember(37), IfTrue(nameof(ReorderPoint)), Prepend(nameof(Strings.QtyDesired)), NoLabel] public decimal QtyDesired { get; set; }
        
        [ProtoMember(19)] public bool CustomIncomeAccount { get; set; }
        [ProtoMember(18), IfTrue(nameof(CustomIncomeAccount)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountInventorySales))] public Guid? IncomeAccount { get; set; }
        [ProtoMember(16)] public bool CustomExpenseAccount { get; set; }
        [ProtoMember(15), IfTrue(nameof(CustomExpenseAccount)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountInventoryPurchases))] public Guid? ExpenseAccount { get; set; }
        [ProtoMember(29), Label(nameof(Strings.Autofill), nameof(Strings.LineDescription))] public bool HasDefaultLineDescription { get; set; }
        [ProtoMember(4), IfTrue(nameof(HasDefaultLineDescription)), NoLabel, Textarea] public string DefaultLineDescription { get; set; }
        [ProtoMember(31), Label(nameof(Strings.Autofill), nameof(Strings.Purchases), nameof(Strings.UnitPrice))] public bool HasDefaultPurchaseUnitPrice { get; set; }
        [ProtoMember(2), IfTrue(nameof(HasDefaultPurchaseUnitPrice)), NoLabel] public decimal DefaultPurchaseUnitPrice { get; set; }
        [ProtoMember(32), Label(nameof(Strings.Autofill), nameof(Strings.Sales), nameof(Strings.UnitPrice))] public bool HasDefaultSalesUnitPrice { get; set; }
        [ProtoMember(3), IfTrue(nameof(HasDefaultSalesUnitPrice)), NoLabel] public decimal DefaultSalesUnitPrice { get; set; }
        [ProtoMember(34), Label(nameof(Strings.Autofill), nameof(Strings.Sales), nameof(Strings.Division))] public bool HasDefaultDivision { get; set; }
        [ProtoMember(35), Autocomplete(typeof(Division)), IfTrue(nameof(HasDefaultDivision)), NoLabel, Short] public Guid? DefaultDivision { get; set; }
        [ProtoMember(30), Label(nameof(Strings.Autofill), nameof(Strings.TaxCode)), IfContains<TaxCode>] public bool HasDefaultTaxCode { get; set; }
        [ProtoMember(12), Autocomplete(typeof(TaxCode)), IfTrue(nameof(HasDefaultTaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [ProtoMember(28)] public bool HideItemNameOnPrintedDocuments { get; set; }
        [Guide("Mark this inventory item as inactive to hide it from dropdown selection lists while preserving all transaction history.")]
        [Guide("Use this for discontinued products or items you no longer sell. Historical transactions and inventory movements remain in reports.")]
        [Guide("You can reactivate an item at any time by unchecking this box.")]
        [ProtoMember(10)] public bool Inactive { get; set; }
        [Guide("Custom fields allow you to track additional inventory information specific to your business.")]
        [Guide("Common uses include manufacturer, supplier part number, warehouse location, bin number, or product specifications.")]
        [Guide("Create custom fields under `Settings` → `Custom Fields` to make them available here.")]
        [ProtoMember(9)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Enhanced custom fields that support different data types like dates, numbers, and dropdown lists. Configure these under `Settings` → `CustomFields`.")]
        [ProtoMember(33)] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<InventoryItem>.CompareTo(InventoryItem other) => (Inactive, ItemCode, ItemName).CompareTo((other.Inactive, other.ItemCode, other.ItemName));
        public bool HasCostOfGoodsSold => true;
        string ICode.Code => ItemCode;

        public decimal? GetQtyDesired()
        {
            if (!ReorderPoint) return null;
            if (QtyDesired <= 0m) return null;
            return QtyDesired;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }

        [ProtoContract]
        public sealed class Obsolete_StartingBalanceQuantity
        {
            [ProtoMember(2)] public decimal Qty { get; set; }
            [ProtoMember(1)] public Guid? InventoryLocation { get; set; }
        }

        [ProtoMember(25)] public int? Obsolete_ProductionStage { get; set; }
        [ProtoMember(26)] public bool Obsolete_TrackQuantityToReceive { get; set; }
        [ProtoMember(27)] public bool Obsolete_TrackQuantityToDeliver { get; set; }
        [ProtoMember(24)] public Obsolete_StartingBalanceQuantity[] Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(6)] public decimal Obsolete_StartingBalanceAverageCost2 { get; set; }
        [ProtoMember(5)] internal bool Obsolete_HasOpeningBalance;
        [ProtoMember(8)] internal DateTime Obsolete_OpeningBalanceDate;
        [ProtoMember(14)] internal bool Obsolete_HasStartingBalance;
        [ProtoMember(22)] internal decimal Obsolete_StartingBalanceCost;
        [ProtoMember(23)] internal decimal Obsolete_StartingBalanceQty;
        [ProtoMember(21)] internal Guid? Obsolete_StartingBalanceInventoryLocation;
        [ProtoMember(7)] internal decimal Obsolete_StartingBalanceQty2;

        public Guid GetAssetAccount()
        {
            if (ControlAccount.HasValue) return ControlAccount.Value;
            else return ManagerServer.Model.Master.AccountKeys.InventoryOnHand;
        }

        public Guid GetExpenseAccount()
        {
            if (CustomExpenseAccount && ExpenseAccount.HasValue) return ExpenseAccount.Value;
            else return ManagerServer.Model.Master.AccountKeys.InventoryPurchases;
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

        Guid IItem.Key => Key;
        public bool HasDefaultQty => true;
        public decimal? DefaultQty => 1m;
        public Guid? PurchaseItemAccount => ControlAccount ?? ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetInventoryOnHandAccount));
        public Guid? PurchaseItemTaxCode => DefaultTaxCode;
        public Guid? PurchaseItemTrackingCode => Division;
        public Guid? SaleItemAccount => (CustomIncomeAccount ? IncomeAccount : null) ?? ManagerServer.Model.Object.GetGuidByType(typeof(ProfitAndLossStatementAccountInventorySales));
        public Guid? SaleItemTaxCode => DefaultTaxCode;
        public Guid? SaleItemTrackingCode => Division;

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

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            if (filter is ControlAccountForInventoryItems && ControlAccount != filter.Key) return false;
            return true;
        }
    }
}

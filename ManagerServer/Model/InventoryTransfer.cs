using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("7eaafddc-54c9-4235-98d2-e8a1ee438150")]
    public sealed class InventoryTransfer : Transaction, IHasAutomaticReference, IComparable<InventoryTransfer>, ICustomFields, ICode, IHasCustomTheme
    {
        [Guide("Enter the date of the inventory transfer. This determines when the stock movement is recorded.")]
        [Guide("The transfer date affects inventory reports and the availability of items at each location.")]
        [ProtoMember(2), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this transfer. This could be a transfer ID or shipment number.")]
        [Guide("References help track physical movements of goods between locations and match with shipping documents.")]
        [ProtoMember(1), Short, Placeholder(nameof(Strings.Optional)), Prepend("#")] public string Reference { get; set; }
        [Guide("Select the source location from which inventory items are being transferred out.")]
        [Guide("The quantity on hand at this location will be reduced by the transferred amounts.")]
        [ProtoMember(3), Autocomplete(typeof(CustomInventoryLocation)), NoWrap, Prepend(nameof(Strings.From))] public Guid? InventoryLocation { get; set; }
        [Guide("Select the destination location to which inventory items are being transferred.")]
        [Guide("The quantity on hand at this location will be increased by the transferred amounts.")]
        [ProtoMember(4), Autocomplete(typeof(CustomInventoryLocation)), Prepend(nameof(Strings.To)), EmptyLabel] public Guid? ToInventoryLocation { get; set; }
        [Guide("Optionally, add a description for this transfer, such as the reason or purpose of moving inventory.")]
        [Guide("Common reasons include stock balancing, fulfilling orders from another location, or consolidating inventory.")]
        [ProtoMember(5), Long] public string Description { get; set; }
        [Guide("Enter the inventory items to transfer. Each line represents a different item and quantity to move.")]
        [Guide("Only items with sufficient quantity at the source location can be transferred.")]
        [ProtoMember(6)] public Line[] Lines { get; set; }
        [ProtoMember(8), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(9), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(7)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(10), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(11)] public CustomFields CustomFields2 { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        [CustomFields]
        [ProtoContract]
        [Guid("43bef636-30a4-4cb1-a13b-0cb5a4717b92")]
        public sealed class Line : ITransactionLine
        {
            [Guide("Select the inventory item to transfer. Only items with quantity on hand at the source location can be selected.")]
            [Guide("The system tracks inventory by location, so items must be available at the 'From' location.")]
            [ProtoMember(9), Autocomplete(typeof(InventoryItem)), OnChangeSetDefault(nameof(LineDescription))] public Guid? Item { get; set; }
            [Guide("Enter a description for this transfer line, such as specific details about the items being moved.")]
            [Guide("You can note serial numbers, batch codes, or special handling instructions for the transferred items.")]
            [ProtoMember(1), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(10)] public CustomFields CustomFields2 { get; set; }
            [Guide("Enter the quantity to transfer. The unit depends on the selected inventory item.")]
            [Guide("The quantity cannot exceed what is available at the source location on the transfer date.")]
            [ProtoMember(6), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName))] public decimal? Qty { get; set; }

            protected override string GetLineDescription() => LineDescription;
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
            return false;
        }

        public override Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (Lines == null) return [];

            var inventoryLocationSource = InventoryLocation.HasValue ? database.SingleOrDefault<CustomInventoryLocation>(InventoryLocation.Value) : null;
            var inventoryLocationDestination = ToInventoryLocation.HasValue ? database.SingleOrDefault<CustomInventoryLocation>(ToInventoryLocation.Value) : null;
            var baseCurrency = database.Single<BaseCurrency>();

            var list = new List<Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in Lines)
            {
                if (!e.Item.HasValue) continue;
                if (!e.Qty.HasValue) continue;
                if (e.Qty == 0m) continue;
                var inventoryItem = database.SingleOrDefault<InventoryItem>(e.Item.Value);
                if (inventoryItem == null) continue;

                list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: Date,
                    generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                    transactionAmount: 0m,
                    transactionCurrency: baseCurrency,
                    qty: e.Qty.Value*-1m,
                    inventoryItem: inventoryItem,
                    inventoryLocation: inventoryLocationSource,
                    transactionLine: e
                ));

                list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: Date,
                    generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                    transactionAmount: 0m,
                    transactionCurrency: baseCurrency,
                    qty: e.Qty.Value,
                    inventoryItem: inventoryItem,
                    inventoryLocation: inventoryLocationDestination,
                    transactionLine: e
                ));
            }

            return list.ToArray();
        }

        int IComparable<InventoryTransfer>.CompareTo(InventoryTransfer other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}

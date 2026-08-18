using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ManagerServer.Query;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("866217a4-f841-47de-a4e6-87152405c88d")]
    public sealed class GoodsReceipt : Transaction, IHasAutomaticReference, IComparable<GoodsReceipt>, ICustomFields, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when goods were received. This determines when inventory quantities are updated.")]
        [ProtoMember(3), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this goods receipt. This could be a delivery note number or internal receiving number.")]
        [ProtoMember(1), NoWrap] public string Reference { get; set; }
        [Guide("Enter the supplier's order number if provided on their delivery documentation.")]
        [ProtoMember(2), NoWrap, Short, IfNotEmpty] public string OrderNumber { get; set; }
        [Guide("Enter the supplier's invoice number if goods are received with an invoice.")]
        [ProtoMember(10), Short, IfNotEmpty] public string InvoiceNumber { get; set; }
        [Guide("Select the supplier from whom goods are being received. This links the receipt to the supplier's account.")]
        [ProtoMember(4), NoWrap, Autocomplete(typeof(Supplier)), Prepend(nameof(Strings.From))] public Guid? Supplier { get; set; }
        [Guide("Select the related purchase order if these goods fulfill an existing order. This helps track order fulfillment.")]
        [ProtoMember(16), NoWrap, IfNotNull(nameof(Supplier)), Short, Autocomplete(typeof(PurchaseOrder), Filter = nameof(Supplier)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.OrderNumber)), TableColumn] public Guid? PurchaseOrder { get; set; }
        [Guide("Select the related purchase invoice if goods are received with an invoice already entered.")]
        [ProtoMember(17), IfNotNull(nameof(Supplier)), Short, Autocomplete(typeof(PurchaseInvoice), Filter = nameof(Supplier)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.InvoiceNumber)), TableColumn] public Guid? PurchaseInvoice { get; set; }
        [Guide("Select the inventory location where received goods will be stored.")]
        [ProtoMember(11), Autocomplete(typeof(CustomInventoryLocation)), Prepend(nameof(Strings.To))] public Guid? InventoryLocation { get; set; }
        [Guide("Optionally, add a description or notes about this goods receipt, such as condition of goods or special handling.")]
        [ProtoMember(6), Long] public string Description { get; set; }
        [Guide("Enter the inventory items received. Each line represents a different item and quantity received.")]
        [ProtoMember(15)] public Line[] Lines { get; set; }
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(22), Label(nameof(Strings.CustomTitle))] public bool HasGoodsReceiptCustomTitle { get; set; }
        [ProtoMember(23), IfTrue(nameof(HasGoodsReceiptCustomTitle)), Placeholder(nameof(Strings.GoodsReceipts)), NoLabel] public string GoodsReceiptCustomTitle { get; set; }
        [ProtoMember(12), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(13), IfTrue(nameof(CustomTheme)), NoLabel, Autocomplete(typeof(CustomTheme))] public Guid? CustomThemeId { get; set; }
        [ProtoMember(14), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(18), Label(nameof(Strings.Footers))] public bool HasGoodsReceiptFooters { get; set; }
        [ProtoMember(19), Autocomplete(typeof(ManagerServer.Model.GoodsReceiptFooter)), NoLabel, IfTrue(nameof(HasGoodsReceiptFooters))] public Guid[] GoodsReceiptFooters { get; set; }
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(21)] public CustomFields CustomFields2 { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        [CustomFields]
        [ProtoContract]
        [Guid("ef68acfa-1a35-49f6-9c74-487a8af1dfdc")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [Guide("Select the inventory item being received. Only items marked as purchased can be selected.")]
            [ProtoMember(1), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(LineDescription))] public Guid? Item { get; set; }
            [Guide("Enter a description for this line item, such as specific details about the items received.")]
            [ProtoMember(2), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(9)] public CustomFields CustomFields2 { get; set; }
            [Guide("Enter the quantity received. This will increase the inventory on hand for the selected item.")]
            [ProtoMember(3), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }

            public override Guid? GetItem() => Item;
            protected override string GetLineDescription() => LineDescription;
            protected override decimal? GetQty() => Qty;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
        }

        [ProtoMember(7)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }

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
            var supplier = database.SingleOrDefault<Supplier>(Supplier);
            if (supplier == null) return [];

            var inventoryLocation = database.SingleOrDefault<CustomInventoryLocation>(InventoryLocation);

            var baseCurrency = database.Single<BaseCurrency>();
            var purchaseOrder = database.SingleOrDefault<PurchaseOrder>(PurchaseOrder);

            var list = new List<Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in Lines)
            {
                var inventoryItem = database.SingleOrDefault<InventoryItem>(e.Item);

                if (e.Qty.HasValue && inventoryItem != null)
                {
                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        transactionAmount: 0m,
                        generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                        transactionLine: e,
                        inventoryLocation: inventoryLocation,
                        transactionCurrency: baseCurrency,
                        purchaseOrder: purchaseOrder,
                        date: Date,
                        supplier: supplier,
                        inventoryItem: inventoryItem,
                        qty: e.Qty.Value
                    ));
                }
                else
                {
                    // This is required for "View" screens so this line shows when no item is selected
                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                transactionAmount: 0m,
                                generalLedgerAccount: database.Single<BalanceSheetSuspenseAccount>(),
                                inventoryItem: inventoryItem,
                                transactionCurrency: baseCurrency,
                                transactionLine: e,
                                date: Date,
                                supplier: supplier,
                                qty: e.Qty
                            ));
                }
            }

            return list.ToArray();
        }

        int IComparable<GoodsReceipt>.CompareTo(GoodsReceipt other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }
    }
}

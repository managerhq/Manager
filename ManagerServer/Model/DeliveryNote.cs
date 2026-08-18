using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete.Obsolete32;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("a0f6a539-f6a4-4a38-a69a-546a608a1f6d")]
    public sealed class DeliveryNote : Transaction, IHasAutomaticReference, IComparable<DeliveryNote>, ICustomFields, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when the goods were delivered or shipped to the customer.")]
        [ProtoMember(3), NoWrap] public DateTime DeliveryDate { get; set; }
        [Guide("Enter a reference number for this delivery note. This could be a delivery note number, shipping reference, or tracking number.")]
        [ProtoMember(1), NoWrap] public string Reference { get; set; }
        [Guide("If this delivery relates to a sales order, enter the order number here. This helps link deliveries to customer orders.")]
        [ProtoMember(2), NoWrap, Short, IfNull(nameof(SalesOrder)), IfNotEmpty] public string OrderNumber { get; set; }
        [Guide("If this delivery relates to a sales invoice, enter the invoice number here. This helps track which invoice covers the delivered goods.")]
        [ProtoMember(10), Short, IfNull(nameof(SalesInvoice)), IfNotEmpty] public string InvoiceNumber { get; set; }
        [Guide("Select the inventory location from which the goods are being shipped. This updates inventory levels at the specified location.")]
        [ProtoMember(11), Autocomplete(typeof(CustomInventoryLocation)), Prepend(nameof(Strings.From))] public Guid? InventoryLocation { get; set; }
        [Guide("Select the customer to whom the goods are being delivered. The customer's address will automatically populate the delivery address field.")]
        [ProtoMember(4), NoWrap, Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(DeliveryAddress)), Prepend(nameof(Strings.To))] public Guid? Customer { get; set; }
        [Guide("Optionally, link this delivery note to a specific sales order. This helps track order fulfillment and automatically populates line items.")]
        [ProtoMember(18), NoWrap, IfNotNull(nameof(Customer)), Short, Autocomplete(typeof(SalesOrder), Filter = nameof(Customer)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.OrderNumber))] public Guid? SalesOrder { get; set; }
        [Guide("Optionally, link this delivery note to a specific sales invoice. This connects the physical delivery with the financial transaction.")]
        [ProtoMember(19), IfNotNull(nameof(Customer)), Short, Autocomplete(typeof(SalesInvoice), Filter = nameof(Customer)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.InvoiceNumber))] public Guid? SalesInvoice { get; set; }
        [Guide("Enter the delivery address where the goods are being shipped. This is automatically filled from the customer record but can be modified.")]
        [ProtoMember(5), Textarea] public string DeliveryAddress { get; set; }
        [Guide("Optionally, add any notes or special instructions about this delivery, such as delivery conditions or handling requirements.")]
        [ProtoMember(6), Long] public string Description { get; set; }
        [Guide("Enter the items being delivered. Each line represents a product with its quantity and description.")]
        [ProtoMember(15)] public Line[] Lines { get; set; }
        [Guide("Check this box to display line numbers on the delivery note. This helps reference specific items in communications.")]
        [ProtoMember(22), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(16), Label(nameof(Strings.CustomTitle))] public bool HasDeliveryNoteCustomTitle { get; set; }
        [ProtoMember(17), IfTrue(nameof(HasDeliveryNoteCustomTitle)), Placeholder(nameof(Strings.DeliveryNote)), NoLabel] public string DeliveryNoteCustomTitle { get; set; }
        [ProtoMember(12), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(13), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(20), Label(nameof(Strings.Footers))] public bool HasDeliveryNoteFooters { get; set; }
        [ProtoMember(21), Autocomplete(typeof(ManagerServer.Model.DeliveryNoteFooter)), NoLabel, IfTrue(nameof(HasDeliveryNoteFooters))] public Guid[] DeliveryNoteFooters { get; set; }
        [ProtoMember(8),] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(23)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(14), DoNotCopy] public bool AutomaticReference { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        [CustomFields]
        [ProtoContract]
        [Guid("50ac9f9d-b5f7-4787-8b12-db520db2ad70")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Autocomplete(typeof(ISaleItem)), OnChangeSetDefault(nameof(LineDescription))] public Guid? Item { get; set; }
            [ProtoMember(2), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(9)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(3), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }

            public override Guid? GetItem() => Item;
            protected override string GetLineDescription() => LineDescription;
            protected override decimal? GetQty() => Qty;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
        }

        [ProtoMember(7)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(9)] public string Obsolete_Notes { get; set; }

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
            var customer = database.SingleOrDefault<Customer>(Customer);
            if (customer == null) return [];

            var baseCurrency = database.Single<BaseCurrency>();

            var inventoryLocation = database.SingleOrDefault<CustomInventoryLocation>(InventoryLocation);
            var salesOrder = database.SingleOrDefault<SalesOrder>(SalesOrder);

            var list = new List<Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in Lines)
            {
                var inventoryItem = database.SingleOrDefault<InventoryItem>(e.Item);
                var inventoryKit = database.SingleOrDefault<InventoryKit>(e.Item);

                if (inventoryItem != null && e.Qty.HasValue)
                {
                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        transactionAmount: 0m,
                        generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                        transactionCurrency: baseCurrency,
                        date: DeliveryDate,
                        inventoryLocation: inventoryLocation,
                        transactionLine: e,
                        customer: customer,
                        inventoryItem: inventoryItem,
                        salesOrder: salesOrder,
                        qty: e.Qty.Value*-1m
                    ));
                }
                else if (inventoryKit != null && e.Qty.HasValue)
                {
                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                transactionAmount: 0m,
                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                transactionCurrency: baseCurrency,
                                transactionLine: e,
                                date: DeliveryDate,
                                customer: customer,
                                inventoryKit: inventoryKit,
                                salesOrder: salesOrder,
                                qty: e.Qty * -1m
                            ));

                    if (inventoryKit.BillOfMaterials != null)
                    {
                        foreach (var e2 in inventoryKit.BillOfMaterials)
                        {
                            var qty = e.Qty.Value * e2.Qty;
                            if (qty <= 0m) continue;
                            var inventoryItemWithinInventoryKit = database.SingleOrDefault<InventoryItem>(e2.InventoryItem);
                            if (inventoryItemWithinInventoryKit == null) continue;

                            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                transactionAmount: 0m,
                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                transactionCurrency: baseCurrency,
                                date: DeliveryDate,
                                customer: customer,
                                inventoryItem: inventoryItemWithinInventoryKit,
                                inventoryLocation: inventoryLocation,
                                inventoryKit: inventoryKit,
                                salesOrder: salesOrder,
                                qty: qty * -1m
                            ));
                        }
                    }
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
                               inventoryKit: inventoryKit,
                               transactionCurrency: baseCurrency,
                               transactionLine: e,
                               date: DeliveryDate,
                               customer: customer,
                               qty: e.Qty * -1m
                           ));
                }
            }

            return list.ToArray();
        }

        int IComparable<DeliveryNote>.CompareTo(DeliveryNote other)
        {
            return (other.DeliveryDate, other.Reference).CompareTo((DeliveryDate, Reference));
        }
    }
}

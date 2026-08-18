using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("d7ff6694-f1ef-419f-8ae2-55527a02e95f")]
    public sealed class InventoryWriteOff : Transaction, IHasAutomaticReference, IComparable<InventoryWriteOff>, ICustomFields, IHasCustomTheme
    {
        [Guide("Enter the date of the inventory write-off. This determines when the inventory adjustment and expense are recorded.")]
        [Guide("The write-off date affects inventory valuation and when the loss appears in your profit and loss statement.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this write-off. This could be an adjustment number or incident report reference.")]
        [Guide("References help track inventory losses and link to supporting documentation like damage reports or disposal certificates.")]
        [ProtoMember(12)] public string Reference { get; set; }
        [Guide("Select the inventory location from which items are being written off. Leave blank to write off from unspecified location.")]
        [Guide("If you track inventory by location, select where the loss occurred. Otherwise, leave blank for general inventory.")]
        [ProtoMember(10), Autocomplete(typeof(CustomInventoryLocation))] public Guid? InventoryLocation { get; set; }
        [Guide("Optionally, add a description explaining the reason for the write-off, such as 'Water damage' or 'Expired products'.")]
        [Guide("Good descriptions provide an audit trail and help identify patterns in inventory losses for better control.")]
        [ProtoMember(2), Long] public string Description { get; set; }
        [Guide("Enter the inventory items to write off. Each line represents a different item and quantity to remove from stock.")]
        [Guide("The system will reduce inventory quantities and record the cost as an expense or capitalize it to an asset.")]
        [ProtoMember(3)] public Item[] Items { get; set; }
        [Guide("Select the account to allocate the write-off expense. This is typically an expense account like 'Inventory write-offs'.")]
        [Guide("You can also select a fixed asset account if the inventory is being used to construct or improve an asset.")]
        [ProtoMember(4), NoWrap, Prepend(nameof(Strings.Account)), Autocomplete(typeof(IInventoryWriteOffAccount))] public Guid? Allocation { get; set; }
        [Guide("If allocating to a fixed asset account, select the specific fixed asset to capitalize the inventory cost.")]
        [Guide("This is used when inventory items are consumed in building or improving a fixed asset rather than being lost.")]
        [ProtoMember(9), NoWrap, IfTrue(nameof(Allocation), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset))] public Guid? FixedAsset { get; set; }
        [Guide("Select a tax code if the write-off has tax implications, such as GST/VAT adjustments.")]
        [Guide("Tax codes ensure proper VAT/GST treatment when inventory with claimable input tax is written off.")]
        [ProtoMember(6), NoWrap, Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
        [Guide("Optionally assign this write-off to a specific project for project cost tracking.")]
        [Guide("Project allocation helps track inventory losses by project and affects project profitability reports.")]
        [ProtoMember(16), NoWrap, Autocomplete(typeof(Project)), Short] public Guid? Project { get; set; }
        [Guide("Optionally assign this write-off to a specific division for divisional reporting.")]
        [Guide("Division allocation helps analyze inventory losses by business segment or location.")]
        [ProtoMember(5), Autocomplete(typeof(Division)), Short] public Guid? Division { get; set; }
        [ProtoMember(13), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(14), IfTrue(nameof(CustomTheme)), NoLabel, Autocomplete(typeof(CustomTheme))] public Guid? CustomThemeId { get; set; }
        [ProtoMember(15), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(7)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(17)] public CustomFields CustomFields2 { get; set; }
        
        [ProtoMember(18)] public bool Obsolete_HasProjects { get; set; }
        [ProtoMember(8)] public JournalEntry Obsolete_JournalEntry { get; set; }
        [ProtoMember(11)] public long? Obsolete_Reference { get; set; }
        [ProtoMember(20)] public bool Obsolete_LegacyEntry { get; set; }
        [ProtoMember(21)] public InventoryWriteOff Obsolete_LegacyEntryInventoryWriteOff { get; set; }

        public override string GetReference() => Reference;

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        [ProtoContract]
        public sealed class Item
        {
            [Guide("Select the inventory item to write off. The current cost of this item will be expensed.")]
            [Guide("The system uses the average cost of the item to calculate the write-off amount.")]
            [ProtoMember(1), Autocomplete(typeof(InventoryItem))] public Guid? InventoryItem { get; set; }
            [Guide("Enter the quantity to write off. This reduces the item's quantity on hand and creates an expense.")]
            [Guide("The quantity written off cannot exceed the current quantity on hand at the specified location.")]
            [ProtoMember(3), AppendValue(nameof(InventoryItem), nameof(ManagerServer.Model.InventoryItem.UnitName))] public decimal Qty { get; set; }

            [ProtoMember(5)] public decimal Obsolete_UnitCost { get; set; }
            [ProtoMember(4)] public Guid? Obsolete_Project { get; set; }
            [ProtoMember(2)] public decimal? Obsolete_Qty { get; set; }
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
            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            var baseCurrency = database.Single<BaseCurrency>();
            var inventoryLocation = database.SingleOrDefault<CustomInventoryLocation>(InventoryLocation);
            var taxCode = database.SingleOrDefault<TaxCode>(TaxCode);
            var trackingCode = database.SingleOrDefault<Division>(Division);
            var project = database.SingleOrDefault<Project>(Project);

            var account = database.SingleOrDefault<ProfitAndLossStatementAccount>(Allocation) as IGeneralLedgerAccount
                ?? database.SingleOrDefault<BalanceSheetAccount>(Allocation) as IGeneralLedgerAccount
                ?? database.Single<ProfitAndLossStatementAccountInventoryWriteOffs>();

            var fixedAsset = database.SingleOrDefault<FixedAsset>(FixedAsset);            

            if (Items != null)
            {
                foreach (var e in Items)
                {
                    var qty = e.Qty * -1m;

                    var inventoryItem = database.SingleOrDefault<InventoryItem>(e.InventoryItem);
                    if (inventoryItem != null)
                    {
                        var inventoryUnitCost = database.FindInventoryUnitCost(inventoryItem.Key, Date);

                        if (inventoryUnitCost != null)
                        {
                            var amount = baseCurrency.Round(qty.SafeMultiply(inventoryUnitCost.UnitCost));
                            //if (amount > 0m) amount = 0m;

                            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                date: Date,
                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                inventoryItem: inventoryItem,
                                inventoryLocation: inventoryLocation,
                                qty: qty,
                                transactionAmount: amount,
                                isCostOfGoodsSold: true,
                                transactionCurrency: baseCurrency,
                                inventoryUnitCost: inventoryUnitCost
                            ));

                            if (amount != 0m)
                            {
                                if (taxCode != null)
                                {
                                    foreach (var e2 in taxCode.CalculateTaxAmounts(amount, baseCurrency.GetDecimalPlaces(), false))
                                    {
                                        list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                            database: database,
                                            transaction: this,
                                            generalLedgerAccount: database.SingleOrDefault<BalanceSheetAccount>(e2.Account) as IGeneralLedgerAccount ?? database.Single<BalanceSheetTaxPayableAccount>(),
                                            transactionAmount: e2.Amount,
                                            transactionCurrency: baseCurrency,
                                            taxCode: taxCode,
                                            isTaxTransaction: true,
                                            date: Date
                                        ));
                                    }
                                }

                                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    transaction: this,
                                    date: Date,
                                    generalLedgerAccount: account,
                                    fixedAsset: fixedAsset,
                                    taxCode: taxCode,
                                    trackingCode: trackingCode,
                                    project: project,
                                    inventoryItem: inventoryItem,
                                    inventoryLocation: inventoryLocation,
                                    qty: qty,
                                    transactionAmount: -list.Sum(x => x.BaseAmount),
                                    isCostOfGoodsSold: true,
                                    transactionCurrency: baseCurrency,
                                    inventoryUnitCost: inventoryUnitCost
                                ));
                            }
                        }
                        else
                        {
                            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                database: database,
                                transaction: this,
                                date: Date,
                                generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                inventoryItem: inventoryItem,
                                inventoryLocation: inventoryLocation,
                                inventoryUnitCost: database.Single<InventoryUnitCost>(),
                                qty: qty,
                                transactionAmount: 0m,
                                isCostOfGoodsSold: true,
                                transactionCurrency: baseCurrency
                            ));
                        }
                    }
                }
            }

            return list.ToArray();
        }

        int IComparable<InventoryWriteOff>.CompareTo(InventoryWriteOff other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}

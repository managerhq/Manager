using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("da996523-e77e-493c-b134-31c6c6ccae4a")]
    public sealed class ProductionOrder : Transaction, IHasAutomaticReference, ICustomFields, IComparable<ProductionOrder>, ICode, IHasCustomTheme
    {
        [Guide("Enter the date of production. This determines when inventory movements and costs are recorded.")]
        [Guide("Raw materials are consumed and finished goods are created on this date, affecting inventory valuations.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this production order. This could be a batch number or production run identifier.")]
        [Guide("References help track production batches for quality control, recalls, and cost analysis purposes.")]
        [ProtoMember(17)] public string Reference { get; set; }
        [Guide("Select the inventory location where the finished goods will be stored after production.")]
        [Guide("Raw materials are consumed from this same location unless you use inventory transfers first.")]
        [ProtoMember(13), Autocomplete(typeof(CustomInventoryLocation))] public Guid? InventoryLocation { get; set; }
        [Guide("Optionally, add a description for this production order, such as product specifications or production notes.")]
        [Guide("Descriptions help document special production requirements, quality standards, or customer specifications.")]
        [ProtoMember(2), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Select the finished inventory item being produced. This is the output of the production process.")]
        [Guide("The system will increase the quantity on hand of this item and calculate its cost based on inputs.")]
        [ProtoMember(4), Autocomplete(typeof(InventoryItem)), NoWrap] public Guid? FinishedInventoryItem { get; set; }
        [Guide("Enter the quantity of finished goods produced. The unit depends on the selected inventory item.")]
        [Guide("This quantity will be added to inventory at a cost calculated from the consumed materials and expenses.")]
        [ProtoMember(18), EmptyLabel, Prepend(nameof(Strings.Qty)), AppendValue(nameof(FinishedInventoryItem), nameof(ManagerServer.Model.InventoryItem.UnitName))] public decimal Qty { get; set; }
        [Guide("Enter the raw materials and components consumed in production. Each line represents an ingredient or component.")]
        [Guide("The cost of these materials will be transferred from raw material inventory to finished goods inventory.")]
        [ProtoMember(3)] public Item[] BillOfMaterials { get; set; }
        [Guide("Check this box to include non-inventory costs (like labor or overhead) in the production cost calculation.")]
        [Guide("This allows you to capitalize direct labor and manufacturing overhead into the cost of finished goods.")]
        [ProtoMember(9)] public bool AddNonInventoryCostIntoProduction { get; set; }
        [Guide("If including non-inventory costs, enter the expense accounts and amounts to add to production cost.")]
        [Guide("These costs are removed from expenses and added to the inventory value of the finished goods produced.")]
        [ProtoMember(10), IfTrue(nameof(AddNonInventoryCostIntoProduction))] public ExpenseItem[] ExpenseItems { get; set; }
        [ProtoMember(14), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(15), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(6)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(19)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(16), DoNotCopy] public bool AutomaticReference { get; set; }

        [ProtoMember(7)] public Guid? Obsolete_ExpenseAccount { get; set; }
        [ProtoMember(8)] public decimal? Obsolete_Amount { get; set; }
        [ProtoMember(12)] public JournalEntry Obsolete_JournalEntry { get; set; }
        [ProtoMember(11)] public int? Obsolete_Reference { get; set; }
        [ProtoMember(5)] public decimal? Obsolete_Qty { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        [ProtoContract]
        public sealed class Item
        {
            [Guide("Select the inventory item to consume in production. This reduces the item's quantity on hand.")]
            [Guide("Only items with sufficient quantity available can be selected for the bill of materials.")]
            [ProtoMember(1), Autocomplete(typeof(InventoryItem))] public Guid? BillOfMaterials { get; set; }
            [Guide("Enter the quantity of this item consumed to produce the finished goods. The unit depends on the selected item.")]
            [Guide("This is the quantity needed per production batch, not per unit of finished goods.")]
            [ProtoMember(3), AppendValue(nameof(BillOfMaterials), nameof(ManagerServer.Model.InventoryItem.UnitName)), EmptyLabel, Prepend(nameof(Strings.Qty))] public decimal Qty { get; set; }

            [ProtoMember(2)] public decimal? Obsolete_Qty { get; set; }
        }

        [ProtoContract]
        public sealed class ExpenseItem
        {
            [Guide("Select the expense account for this production cost, such as direct labor or manufacturing overhead.")]
            [Guide("Common accounts include direct labor, factory rent, utilities, and other manufacturing costs.")]
            [ProtoMember(1), Autocomplete(typeof(ICustomGeneralLedgerAccount))] public Guid? Account { get; set; }
            [Guide("Enter the amount of this expense to allocate to the production cost. This increases the unit cost of finished goods.")]
            [Guide("The total amount is spread across all units produced, increasing their inventory value.")]
            [ProtoMember(2), Sum] public decimal Amount { get; set; }
            [Guide("Optionally assign this expense to a specific division for cost allocation and reporting.")]
            [Guide("Division tracking helps analyze production costs by business segment or production facility.")]
            [ProtoMember(3), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(ICustomGeneralLedgerAccount.DivisionEnabled)), Short] public Guid? Division { get; set; }
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

            var outputInventoryItem = database.SingleOrDefault<InventoryItem>(FinishedInventoryItem);

            var outputQty = Qty;

            if (BillOfMaterials != null)
            {
                foreach (var e in BillOfMaterials)
                {
                    var inventoryItem = database.SingleOrDefault<InventoryItem>(e.BillOfMaterials);
                    if (inventoryItem != null)
                    {
                        if (inventoryItem.Key == FinishedInventoryItem)
                        {
                            outputQty -= e.Qty;
                        }
                        else
                        {
                            var inventoryUnitCost = database.FindInventoryUnitCost(inventoryItem.Key, Date);

                            if (inventoryUnitCost != null)
                            {
                                var amount = baseCurrency.Round(e.Qty * inventoryUnitCost.UnitCost);

                                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    transaction: this,
                                    date: Date,
                                    generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                    inventoryItem: inventoryItem,
                                    inventoryLocation: inventoryLocation,
                                    qty: e.Qty * -1m,
                                    transactionAmount: amount * -1m,
                                    isCostOfGoodsSold: true,
                                    transactionCurrency: baseCurrency,
                                    inventoryUnitCost: inventoryUnitCost
                                ));
                            }
                            else
                            {
                                list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                                    database: database,
                                    transaction: this,
                                    date: Date,
                                    generalLedgerAccount: database.Single<BalanceSheetInventoryOnHandAccount>(),
                                    inventoryItem: inventoryItem,
                                    inventoryUnitCost: database.Single<InventoryUnitCost>(),
                                    inventoryLocation: inventoryLocation,
                                    qty: e.Qty * -1m,
                                    transactionAmount: 0m,
                                    isCostOfGoodsSold: true,
                                    transactionCurrency: baseCurrency));
                            }
                        }
                    }
                }
            }

            //var anyInputItems = list.Any();

            if (AddNonInventoryCostIntoProduction && ExpenseItems != null)
            {
                foreach (var e in ExpenseItems)
                {
                    var profitAndLossStatementAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(e.Account) as IGeneralLedgerAccount;
                    var balanceSheetAccount = database.SingleOrDefault<BalanceSheetAccount>(e.Account) as IGeneralLedgerAccount;
                    var account = profitAndLossStatementAccount ?? balanceSheetAccount ?? database.Single<BalanceSheetSuspenseAccount>();

                    list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: Date,
                            generalLedgerAccount: account,
                            transactionAmount: e.Amount * -1m,
                            transactionCurrency: baseCurrency,
                            trackingCode: database.SingleOrDefault<Division>(e.Division)
                        ));
                }
            }

            var generalLedgerAccount = database.Single<BalanceSheetInventoryOnHandAccount>() as IGeneralLedgerAccount;
            //if (anyInputItems) generalLedgerAccount = database.Single<BalanceSheetProductionInProgressAccount>();
            if (outputInventoryItem == null) generalLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();

            var total = list.Sum(x => x.BaseAmount);

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: Date,
                    generalLedgerAccount: generalLedgerAccount,
                    inventoryItem: outputInventoryItem,
                    inventoryLocation: inventoryLocation,
                    contraTransactions: list.ToArray(),
                    qty: outputQty,
                    isBalancing: true,
                    transactionAmount: total*-1m,
                    transactionCurrency: baseCurrency
                ));

            return list.ToArray();
        }

        int IComparable<ProductionOrder>.CompareTo(ProductionOrder other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }
    }
}

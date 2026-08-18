using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryWriteOff))]
    [Guide("The *Inventory Write-off* view displays comprehensive details about a specific inventory write-off transaction, including the date, reference number, and description.")]
    [Guide("The view presents a table showing all inventory items being written off, with their *item codes*, *item names*, and *quantities*.")]
    [Guide("You can click the **Edit** button to modify the write-off details, change quantities, add or remove items, or update the reference and description.")]
    [LinkGuide("For more information, see:", typeof(InventoryWriteOffForm))]
    internal sealed class InventoryWriteOffView : TransactionView<ManagerServer.Model.InventoryWriteOff>
    {
        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new InventoryWriteOffTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
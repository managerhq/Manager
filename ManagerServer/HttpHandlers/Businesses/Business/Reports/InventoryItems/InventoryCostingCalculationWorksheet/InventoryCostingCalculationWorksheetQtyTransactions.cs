using ManagerServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryCostingCalculationWorksheet), nameof(Strings.Qty), nameof(Strings.Transactions))]
    [Guide("Shows quantity transactions affecting inventory items for cost calculations.")]
    [Guide("Displays receipts, deliveries, and adjustments that impact inventory quantities.")]
    internal sealed class InventoryCostingCalculationWorksheetQtyTransactions : Summary.BaseGeneralLedgerTransactionsInheritable
    {
    }
}
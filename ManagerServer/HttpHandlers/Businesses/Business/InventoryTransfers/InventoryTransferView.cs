using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryTransfer))]
    [Guide("The *inventory transfer* view displays detailed information about a completed transfer of inventory items between locations.")]
    [Guide("This view shows the *transfer date*, *reference number*, source and destination *inventory locations*, and all items that were transferred including their quantities.")]
    [Guide("From this view, you can review the transfer details, click **Edit** if corrections are needed, or click **Copy to** to create a new transfer with similar information.")]
    [Guide("The view organizes information in a clear format with the transfer header details at the top, followed by a table listing all transferred items with their *item codes*, descriptions, quantities, and any *custom field* values.")]
    [LinkGuide("To learn how to create or edit inventory transfers, see:", typeof(InventoryTransferForm))]
    internal sealed class InventoryTransferView : TransactionView<ManagerServer.Model.InventoryTransfer>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction)];
        }
    }
}
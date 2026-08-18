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

namespace ManagerServer.HttpHandlers.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    [Title(nameof(Strings.GoodsReceipt))]
    [Guide("The **Goods Receipt** view displays comprehensive details about items received into inventory from suppliers.")]
    [Guide("A *goods receipt* documents the physical receipt of inventory items, allowing you to track what has been delivered against *purchase orders* or *purchase invoices*.")]
    [Guide("From this view, you can edit receipt details, print the document for your records, or verify the quantities and items received against your purchase documentation.")]
    [Guide("The view shows key information including the *receipt date*, *reference number*, related *purchase order* or *invoice numbers*, *inventory location*, and *supplier details*.")]
    [LinkGuide("To learn how to create or edit goods receipts, see:", typeof(GoodsReceiptForm))]
    internal sealed class GoodsReceiptView : TransactionView<ManagerServer.Model.GoodsReceipt>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.Transaction)];
        }
    }
}
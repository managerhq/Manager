using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.GoodsReceipts
{
    [ProtoContract]
    [Title(nameof(Strings.GoodsReceipt), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays a preview of your *goods receipt footer* as it will appear on printed or emailed goods receipts.")]
    [Guide("Use this view to verify that your footer information is formatted correctly and contains all necessary details before applying it to your goods receipts.")]
    [LinkGuide("For more information, see:", typeof(GoodsReceiptFooterForm))]
    internal class GoodsReceiptFooterView : DefaultView<GetGoodsReceiptFooterView>
    {
    }
}

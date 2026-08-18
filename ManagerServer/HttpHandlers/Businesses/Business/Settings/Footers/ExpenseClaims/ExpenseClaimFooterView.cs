using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.ExpenseClaims
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaim), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays the current *expense claim footer* that appears at the bottom of your expense claim forms.")]
    [Guide("The footer is displayed exactly as it will appear on printed or emailed expense claims, allowing you to preview the layout and formatting.")]
    [Guide("To modify the footer content or formatting, click the **Edit** button.")]
    [LinkGuide("For more information, see:", typeof(ExpenseClaimFooterForm))]
    internal class ExpenseClaimFooterView : DefaultView<GetExpenseClaimFooterView>
    {
    }
}

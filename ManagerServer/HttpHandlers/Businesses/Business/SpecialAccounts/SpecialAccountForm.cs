using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.SpecialAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.SpecialAccount), nameof(Strings.Edit))]
    [Guide("The `SpecialAccount` form enables you to create custom subsidiary ledgers for tracking unique financial relationships or obligations that don't fit into standard accounting categories.")]
    [Guide("Special accounts provide flexibility to track items such as loans (given or received), customer deposits, escrow accounts, project funds, or any other financial position requiring separate monitoring. Each special account maintains its own balance and transaction history, appearing as a line item within its designated control account on the balance sheet.")]
    [Guide("When creating a special account, choose a meaningful code and name that clearly identifies its purpose. Select the appropriate control account to determine where it appears on your balance sheet. You can also assign divisions for segmented reporting and set a specific currency if dealing with foreign currency positions. Special accounts are particularly useful for businesses needing to track multiple loans, project-specific funds, or customer retainers separately.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.SpecialAccount))]
    internal sealed class SpecialAccountForm : NakedVueForm<SpecialAccount>
    {
    }
}
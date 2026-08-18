using ManagerServer.Api.Businesses.Business.Settings.CustomerPortals;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomerPortals
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerPortal))]
    [Guide("The customer portal provides your customers with secure online access to view their account information, invoices, and statements.")]
    [Guide("This view displays the current configuration of your customer portal, including the unique portal URL that customers will use to access their accounts.")]
    [Guide("From here, you can see the authentication method configured for customer access and verify that the portal is properly set up.")]
    [LinkGuide("To modify portal settings, see:", typeof(CustomerPortalForm))]
    internal sealed class CustomerPortalView : DefaultView<GetCustomerPortalView>
    {
    }
}

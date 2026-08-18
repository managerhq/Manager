using ManagerServer.Api.Businesses.Business.Settings.AccessTokens;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.AccessTokens
{
    [ProtoContract]
    [Title(nameof(Strings.AccessToken))]
    [Guide("Access tokens provide secure authentication for external applications to connect with your business data through the API.")]
    [Guide("This view displays the endpoint URL and secret key required for API integration. The endpoint URL is the address your application will use to send API requests, while the secret key authenticates those requests.")]
    [Guide("When you first view an access token, the system generates a unique secret key automatically. **Important**: Copy and save the secret key immediately - for security reasons, it will only be displayed once. After you navigate away from this screen, the secret will be permanently hidden and shown only as asterisks.")]
    [LinkGuide("To learn how to create and manage access tokens, see:", typeof(AccessTokenForm))]
    internal sealed class AccessTokenView : DefaultView<GetAccessTokenView>
    {
    }
}

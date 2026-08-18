using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.AccessTokens
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("c4711963-0e6b-4ef6-9c10-c10b645b57fc")]
    [Title(nameof(Strings.AccessTokens))]
    [Guide("Access tokens allow you to connect Manager with other software or automate tasks using the API. You can find this feature in the **Settings** tab under **Access Tokens**.")]
    [SettingsItemScreenshot("fa-robot", nameof(Strings.AccessTokens))]
    [Guide("Click the **New Access Token** button to create a new *access token*.")]
    [HeroButtonScreenshot(nameof(Strings.AccessTokens), nameof(Strings.NewAccessToken))]
    [Guide("Once you have obtained the *access token*, you can use it to authenticate with the Manager API.")]
    [Guide("To see all available API endpoints, click the **API** button in the bottom-right corner of the screen.")]
    [SmallBottomButtonScreenshot("API")]
    internal sealed class AccessTokens : NakedObjectsWithAutomaticRows<ManagerServer.Model.AccessToken>
    {
        [Default]
        [Guid("dff37ee8-091a-4b22-a3f9-71f15ac6c559")]
        public string[] GetName(ManagerServer.Model.AccessToken[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: "/swagger", @class: "btn btn-xs")) Write("API");
        }
    }
}

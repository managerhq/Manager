using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.UserPermissions
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.UserPermissions))]
    [NewButton(nameof(Strings.NewUserPermissions))]
    [Guide("If you're using the cloud or server edition, you can adjust the access levels for *restricted users* within this specific business file by navigating to the **User Permissions** section under the **Settings** tab.")]
    [SettingsItemScreenshot("fa-user-lock", nameof(Strings.UserPermissions))]
    [Guide("Typically, you don't need to access this screen directly. You can access user permissions across all users and all businesses in a consolidated view from the **Users** tab.")]
    [LinkGuide("For more information, see:", typeof(Users.Users))]
    internal sealed class UserPermissions : PersistentObjectTable<ManagerServer.Model.UserPermissions>
    {
        [Guid("78a8b0ce-780d-41d5-827b-f1962c95397a")]
        public string GetUsername(ManagerServer.Model.UserPermissions o) => o.Username;

        [Guid("88aa9fab-63df-4229-829e-90779d177601")]
        public ManagerServer.Model.Enums.UserPermissionsAccessType GetAccessType(ManagerServer.Model.UserPermissions o) => o.AccessType;
    }
}

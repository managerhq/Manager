using Microsoft.AspNetCore.Http;

namespace ManagerServer.Authentication
{
    public static class ManagerUserExtensions
    {
        private const string Key = "ManagerUser";

        public static void SetManagerUser(this HttpContext context, UserRecord user)
        {
            context.Items[Key] = user;
        }

        public static UserRecord GetManagerUser(this HttpContext context)
        {
            return context.Items.TryGetValue(Key, out var user) ? user as UserRecord : null;
        }
    }
}

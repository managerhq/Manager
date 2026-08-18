using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ManagerServer;
using ManagerServer.Model;
using ManagerServer.Helpers;
using Microsoft.AspNetCore.Http;

namespace ManagerServer.Authentication
{
    public class UserResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = await ResolveUserAsync(context);
            if (user != null)
            {
                context.SetManagerUser(user);
            }

            await _next(context);
        }

        private static async Task<UserRecord> ResolveUserAsync(HttpContext context)
        {
            if (Edition.IsDesktop)
            {
                return ApplicationData.Instance.Users.GetDesktopUser();
            }

            if (context.Request.Cookies["session"] != null)
            {
                var session = context.Request.Cookies["session"].ToString();
                if (!string.IsNullOrWhiteSpace(session))
                {
                    var userRecord = await ManagerServer.ApplicationData.Instance.Users.GetBySessionAsync(session);
                    if (userRecord != null)
                    {
                        return userRecord;
                    }                    
                }
            }

            if (!string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
            {
                var authorization = context.Request.Headers["Authorization"].ToString().Split(' ');
                if (authorization[0] == "Basic")
                {
                    var usernamePassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authorization[1])).Split(':');
                    var username = usernamePassword[0];
                    var password = usernamePassword[1];

                    var record = await ApplicationData.Instance.Users.GetByUsernameAsync(username);

                    if (record != null)
                    {
                        if (record.Verify(password, null))
                        {
                            return record;
                        }
                    }
                }
            }

            return null;
        }
    }
}

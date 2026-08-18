using System;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Helpers;
using Microsoft.AspNetCore.Http;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Logout : HttpHandler
    {
        public override Task Get()
        {
            if (Request.Cookies["session"] != null)
            {
                var cookie = Request.Cookies["session"].ToString();
                if (cookie != null && !string.IsNullOrWhiteSpace(cookie))
                {
                    var userCookie = UserCookie.Deserialize(cookie);
                    if (userCookie != null)
                    {
                        if (userCookie.OnBehalfOf != null)
                        {
                            userCookie.OnBehalfOf = null;
                            Response.Cookies.Append("session", userCookie.Serialize(), new CookieOptions() { Expires = DateTime.UtcNow.AddYears(1), HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax });
                            Response.Redirect(new Users.Users().ToUrl());
                            return Task.CompletedTask;
                        }
                        else
                        {
                            userCookie.UserSession = Guid.Empty;
                            Response.Cookies.Append("session", userCookie.Serialize(), new CookieOptions() { Expires = DateTime.UtcNow.AddYears(1), HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax });
                            Response.Redirect("/");
                            return Task.CompletedTask;
                        }
                    }                    
                }
            }

            Response.Cookies.Delete("session");
            Response.Redirect("/");
            return Task.CompletedTask;
        }
    }
}
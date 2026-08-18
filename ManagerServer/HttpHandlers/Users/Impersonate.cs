using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ManagerServer.Helpers;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Users
{
    [ProtoContract]
    internal sealed class Impersonate : HttpHandler
    {
        [ProtoMember(1)] public string Username;

        public override async Task Get()
        {
            var currentUser = this.GetCurrentUser();

            if (currentUser.Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }

            var userRecord = await ApplicationData.Users.GetByUsernameAsync(Username);
            if (userRecord == null)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }

            var userCookie = UserCookie.Deserialize(Request.Cookies["session"].ToString());
            userCookie.OnBehalfOf = Username;
            var session = userCookie.Serialize();

            Response.Cookies.Append("session", session, new Microsoft.AspNetCore.Http.CookieOptions() { Expires = DateTime.UtcNow.AddYears(1), HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax });
            Response.Redirect(new Businesses.Businesses().ToUrl());
        }
    }
}

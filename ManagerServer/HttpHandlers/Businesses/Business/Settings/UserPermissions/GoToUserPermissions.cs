using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.UserPermissions
{
    [ProtoContract]
    public sealed class GoToUserPermissions : BusinessHandler
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public string Referrer;

        public override Task Get()
        {
            var database = ApplicationData.Businesses.Get(Business);
            if (database == null)
            {
                Response.Redirect(new Users.Users().ToUrl());
                return Task.CompletedTask;
            }
            var userPermissions = database.OfType<ManagerServer.Model.UserPermissions>().Where(x => x.Username == Username).OrderBy(x => x.Key).FirstOrDefault();
            if (userPermissions == null)
            {
                userPermissions = new ManagerServer.Model.UserPermissions() { Key = Guid.CreateVersion7(), Username = Username };
                ApplicationData.Businesses.Process(Business, userPermissions, GetUserName());
            }
            Response.Redirect(new UserPermissionsForm() { Business = Business, Key = userPermissions.Key, Referrer = Referrer }.ToUrl());
            return Task.CompletedTask;
        }
    }
}

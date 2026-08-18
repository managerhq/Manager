using ManagerServer.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class DeleteObjectEndpoint<T> : AuthorizedEndpoint<bool> where T : Model.Object, new()
    {
        public Guid Key { get; set; }

        public override bool AuthorizedHandle()
        {
            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, Key, user.Username);
            return true;
        }
    }
}

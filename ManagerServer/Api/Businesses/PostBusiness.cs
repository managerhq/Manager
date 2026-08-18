using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses
{
    [ProtoContract]
    internal class PostBusiness : AuthenticatedEndpoint<bool>
    {
        public string Name { get; set; }

        public override bool AuthenticatedHandle()
        {
            GetApplicationData().Businesses.CreateAsync(Name).GetAwaiter().GetResult();
            return true;
        }
    }
}

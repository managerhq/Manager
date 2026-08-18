using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class GetObjectEndpoint<T> : AuthorizedEndpoint<T> where T : Model.Object, new()
    {
        [ProtoMember(1)] public Guid Key { get; set; }

        public override T AuthorizedHandle()
        {
            return GetApplicationData().Businesses.Get(Business).SingleOrDefault<T>(Key);
        }
    }
}

using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ManagerServer.Api
{
    internal class AuthenticatedEndpoint<T> : HydratedEndpoint<T>
    {
        public ApplicationData GetApplicationData() => Context.RequestServices.GetRequiredService<ApplicationData>();

        public sealed override T HydratedHandle()
        {
            var user = Context.GetManagerUser();
            if (user == null)
            {
                throw new UnauthenticatedException();
            }
            return AuthenticatedHandle();
        }

        public virtual T AuthenticatedHandle()
        {
            return default;
        }
    }
}

using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class PutObjectEndpoint<T> : AuthorizedEndpoint<bool> where T : Model.Object, new()
    {
        public Guid Key { get; set; }
        public T Value { get; set; }

        public override bool AuthorizedHandle()
        {
            if (Value == null)
            {
                throw new BadRequestException($"Request body is missing a '{nameof(Value)}' property containing the {typeof(T).Name} to update.");
            }
            Value.Key = Key;
            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, Value, user.Username);
            return true;
        }
    }
}

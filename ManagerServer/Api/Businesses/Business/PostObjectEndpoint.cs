using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class PostObjectEndpoint<T> : AuthorizedEndpoint<Guid> where T : Model.Object, new()
    {
        public T Value { get; set; }

        public override Guid AuthorizedHandle()
        {
            if (Value == null)
            {
                throw new BadRequestException($"Request body is missing a '{nameof(Value)}' property containing the {typeof(T).Name} to create.");
            }
            Value.Key = Guid.CreateVersion7();
            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, Value, user.Username);
            return Value.Key;
        }
    }
}

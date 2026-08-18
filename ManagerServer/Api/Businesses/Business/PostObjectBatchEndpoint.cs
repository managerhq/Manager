using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class PostObjectBatchEndpoint<T> : AuthorizedEndpoint<Guid[]> where T : Model.Object, new()
    {
        public T[] Values { get; set; }

        public override Guid[] AuthorizedHandle()
        {
            if (Values == null)
            {
                throw new BadRequestException($"Request body is missing a '{nameof(Values)}' property containing the {typeof(T).Name} batch to create.");
            }
            foreach (var e in Values) e.Key = Guid.CreateVersion7();
            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, Values, user.Username);
            return Values.Select(x => x.Key).ToArray();
        }
    }
}
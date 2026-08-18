using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class PutObjectBatchEndpoint<T> : AuthorizedEndpoint<bool> where T : Model.Object, new()
    {
        public KeyValuePair<Guid, T>[] Values { get; set; }

        public override bool AuthorizedHandle()
        {
            if (Values == null)
            {
                throw new BadRequestException($"Request body is missing a '{nameof(Values)}' property containing the {typeof(T).Name} batch to update.");
            }            

            var actions = new List<ApplicationData.Action>();
            foreach (var e in Values)
            {
                if (e.Value == null)
                {
                    actions.Add(new ApplicationData.DeleteAction(e.Key));
                }
                else
                {
                    e.Value.Key = e.Key;
                    actions.Add(new ApplicationData.CreateOrUpdateAction(e.Value));
                }
            }

            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, actions.ToArray(), user.Username);
            return true;
        }
    }
}

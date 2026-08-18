using ManagerServer.Authentication;
using ManagerServer.Endpoints;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class PutSingletonObjectEndpoint<T> : AuthorizedEndpoint<bool> where T : Model.Object, new()
    {
        public T Value { get; set; }

        public override bool AuthorizedHandle()
        {
            if (Value == null)
            {
                throw new BadRequestException($"Request body is missing a '{nameof(Value)}' property containing the {typeof(T).Name} to update.");
            }
            Value.Key = Model.Object.GetGuidByType(typeof(T));
            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, Value, user.Username);
            return true;
        }
    }
}

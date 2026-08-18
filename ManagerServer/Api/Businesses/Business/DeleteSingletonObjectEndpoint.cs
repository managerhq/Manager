using ManagerServer.Authentication;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class DeleteSingletonObjectEndpoint<T> : AuthorizedEndpoint<bool> where T : Model.Object, new()
    {
        public override bool AuthorizedHandle()
        {
            var key = Model.Object.GetGuidByType(typeof(T));
            var user = Context.GetManagerUser();
            GetApplicationData().Businesses.Process(Business, key, user.Username);
            return true;
        }
    }
}

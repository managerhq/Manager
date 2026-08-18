namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class GetSingletonObjectEndpoint<T> : AuthorizedEndpoint<T> where T : Model.Object, new()
    {
        public override T AuthorizedHandle()
        {
            return GetApplicationData().Businesses.Get(Business).Single<T>();
        }
    }
}

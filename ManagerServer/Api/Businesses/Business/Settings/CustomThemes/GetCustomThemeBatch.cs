namespace ManagerServer.Api.Businesses.Business.Settings.Themes
{
    [ProtoContract]
    internal sealed class GetCustomThemeBatch : GetObjectBatchEndpoint<Model.CustomTheme, GetCustomTheme, PostCustomTheme, PutCustomTheme, DeleteCustomTheme>
    {
    }
}

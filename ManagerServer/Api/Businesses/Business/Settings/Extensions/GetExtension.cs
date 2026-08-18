namespace ManagerServer.Api.Businesses.Business.Settings.Extensions
{
    [ProtoContract]
    [Obsolete("Renamed to GetCustomButton. Kept for backwards compatibility with existing integrations.")]
    internal sealed class GetExtension : GetObjectEndpoint<Model.CustomButton>
    {
    }
}

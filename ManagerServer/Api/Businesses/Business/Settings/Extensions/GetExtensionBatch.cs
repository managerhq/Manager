namespace ManagerServer.Api.Businesses.Business.Settings.Extensions
{
    [ProtoContract]
    [Obsolete("Renamed to GetCustomButtonBatch. Kept for backwards compatibility with existing integrations.")]
    internal sealed class GetExtensionBatch : GetObjectBatchEndpoint<Model.CustomButton, GetExtension, PostExtension, PutExtension, DeleteExtension>
    {
    }
}

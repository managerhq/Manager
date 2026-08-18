namespace ManagerServer.Api.Businesses.Business.Settings.CustomButtons
{
    [ProtoContract]
    internal sealed class GetCustomButtonBatch : GetObjectBatchEndpoint<Model.CustomButton, GetCustomButton, PostCustomButton, PutCustomButton, DeleteCustomButton>
    {
    }
}

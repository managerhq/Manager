namespace ManagerServer.Api.Businesses.Business.Settings.CustomFields.TextCustomFields
{
    [ProtoContract]
    internal sealed class GetTextCustomFieldBatch : GetObjectBatchEndpoint<Model.TextCustomField, GetTextCustomField, PostTextCustomField, PutTextCustomField, DeleteTextCustomField>
    {
    }
}

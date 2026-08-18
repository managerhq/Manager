namespace ManagerServer.Api.Businesses.Business.Settings.CustomFields.DateCustomFields
{
    [ProtoContract]
    internal sealed class GetDateCustomFieldBatch : GetObjectBatchEndpoint<Model.DateCustomField, GetDateCustomField, PostDateCustomField, PutDateCustomField, DeleteDateCustomField>
    {
    }
}

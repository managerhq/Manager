namespace ManagerServer.Api.Businesses.Business.Settings.Divisions
{
    [ProtoContract]
    internal sealed class GetDivisionBatch : GetObjectBatchEndpoint<Model.Division, GetDivision, PostDivision, PutDivision, DeleteDivision>
    {
    }
}

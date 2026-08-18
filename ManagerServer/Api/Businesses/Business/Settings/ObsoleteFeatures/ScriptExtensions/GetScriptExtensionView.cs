namespace ManagerServer.Api.Businesses.Business.Settings.ObsoleteFeatures.ScriptExtensions
{
    [ProtoContract]
    internal sealed class GetScriptExtensionView : GetObjectViewEndpoint<Model.ScriptExtension>
    {
        protected override View Build(Database business, Model.ScriptExtension obj)
        {
            return new View { Title = obj.Name };
        }
    }
}

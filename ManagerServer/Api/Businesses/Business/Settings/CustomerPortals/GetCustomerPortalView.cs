using ManagerServer.Api;
using ManagerServer.Globalization;

namespace ManagerServer.Api.Businesses.Business.Settings.CustomerPortals
{
    [ProtoContract]
    internal sealed class GetCustomerPortalView : GetObjectViewEndpoint<Model.CustomerPortal>
    {
        protected override View Build(Database business, Model.CustomerPortal obj)
        {
            var view = BuildFromMembers(business, obj);

            var portalUrl = new ManagerServer.HttpHandlers.CustomerPortal.Summary.CustomerPortal { CustomerPortal = Key.Value, Business = Business }.ToUrl();
            view.Fields.Add(new View.FieldInfo
            {
                Label = Strings.CustomerPortal,
                Text = "Go to Customer Portal",
                Link = new View.LinkInfo() { Url = portalUrl },
            });

            return view;
        }
    }
}

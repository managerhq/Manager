using System;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    public abstract class BusinessHandler : HttpHandler
    {
        [InheritedProtoMember(100)] public string Business;

        public override System.Threading.Tasks.Task ProcessRequest()
        {
            // Login itself is enforced by the base handler. Here the user must also be assigned to the business.
            var user = GetCurrentUser();
            if (user != null && !(this is Settings.BusinessDetails.BusinessLogoView) && !HasBusinessAccess(user))
            {
                Response.StatusCode = 403;
                return System.Threading.Tasks.Task.CompletedTask;
            }

            return base.ProcessRequest();
        }

        private bool HasBusinessAccess(UserRecord user)
        {
            if (Edition.IsDesktop) return true;
            if (user.Type == ManagerServer.Model.UserType.Administrator) return true;
            return Business != null && user.Businesses != null && user.Businesses.Contains(Business);
        }
    }
}
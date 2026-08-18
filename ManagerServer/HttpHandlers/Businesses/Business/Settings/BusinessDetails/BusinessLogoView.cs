using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BusinessDetails
{
    [ProtoContract]
    public sealed class BusinessLogoView : BusinessHandler
    {
        [ProtoMember(1)] public long Timestamp;

        public override Task Get()
        {
            var database = ApplicationData.Businesses.Get(Business);
            if (database == null) return Task.CompletedTask;
            var businessDetails = database.Single<ManagerServer.Model.BusinessDetails>();
            var businessLogo = ApplicationData.Businesses.GetImage(Business, businessDetails.Key);
            if (businessLogo == null) return Task.CompletedTask;
            if (businessLogo.Item1 == null || businessLogo.Item1.Length == 0) return Task.CompletedTask;

            Response.ContentType = businessLogo.Item2;
            var dateFormat = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
            Response.Headers["Cache-Control"] = "public, max-age=31536000";
            Response.Headers["Date"] = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString(dateFormat, CultureInfo.InvariantCulture);
            return Response.Body.WriteAsync(businessLogo.Item1, 0, businessLogo.Item1.Length);
        }
    }
}
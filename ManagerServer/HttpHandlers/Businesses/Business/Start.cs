using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ProtoBuf;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.Summary))]
    [Guide("The `Summary` page provides a comprehensive overview of your business data and serves as your main dashboard.")]
    [Guide("From here, you can quickly access all areas of your business, view important notifications, and navigate to frequently used features.")]
    [Guide("The system automatically directs you to the first available tab based on your user permissions and business configuration.")]
    [Guide("If there are any issues with your business file, such as corruption or compatibility problems, you will be redirected to the appropriate error page with instructions on how to proceed.")]
    internal sealed class Start : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            if (Business == null || !ApplicationData.Businesses.Exists(Business))
            {
                using (Script()) Write("window.location.href = " + new Businesses().ToUrl().EncodeJsString() + @";");
                return;
            }

            var database = ApplicationData.Businesses.Get(Business);
            if (database == null)
            {
                using (Script()) Write("window.location.href = " + new Businesses().ToUrl().EncodeJsString() + @";");
                return;
            }
            if (database.Status == ManagerServer.Database.DatabaseStatus.Corrupted)
            {
                using (Script()) Write("window.location.href = " + new Corrupt() { Business = Business }.ToUrl().EncodeJsString());
                return;
            }
            if (database.Status == ManagerServer.Database.DatabaseStatus.Invalid)
            {
                using (Script()) Write("window.location.href = " + new Invalid() { Business = Business }.ToUrl().EncodeJsString());
                return;
            }
            if (database.Status == ManagerServer.Database.DatabaseStatus.Incompatible)
            {
                using (Script()) Write("window.location.href = " + new NewerVersionRequired() { Business = Business }.ToUrl().EncodeJsString());
                return;
            }
            if (database.Status == ManagerServer.Database.DatabaseStatus.OutOfMemory)
            {
                using (Script()) Write("window.location.href = " + new NotEnoughMemory() { Business = Business }.ToUrl().EncodeJsString());
                return;
            }

            var obsoleteInventoryCostCalculation = database.Single<ObsoleteInventoryCostCalculation>();
            if (obsoleteInventoryCostCalculation.Enabled && !database.UnorderedOfType<InventoryUnitCost>().Any())
            {
                var inventoryUnitCosts = new ManagerServer.Api.Businesses.Business.Settings.InventoryUnitCosts.GetRecalculatedInventoryUnitCosts() { Business = Business, FromDate = DateTime.MinValue, Context = HttpContext }.AuthorizedHandle();
                ApplicationData.Businesses.Process(Business, inventoryUnitCosts.ToArray(), null);
                ApplicationData.Businesses.Process(Business, obsoleteInventoryCostCalculation.Key, null);
            }

            var firstTab = new Business.Summary.SummaryView() { Business = Business, HttpContext = this.HttpContext }.GetTabs(applyUserPermissions: true).GetAll().FirstOrDefault(x => x.Visible);
            if (firstTab != null)
            {
                using (Script()) Write("window.location.href = " + firstTab.HttpHandler.ToUrl().EncodeJsString());
                return;
            }

            using (Script()) Write("window.location.href = " + new NoTab().ToUrl().EncodeJsString());
        }
    }
}
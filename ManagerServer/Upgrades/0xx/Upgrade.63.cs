using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade63(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var obsoleteSalesInvoiceTemplate = objects.OfType<Model.Obsolete.Obsolete09.SalesInvoiceTemplate09>().SingleOrDefault(x => x.Key == new Guid("0b96900e-552d-4d14-a070-9086e44f188d"));
            if (obsoleteSalesInvoiceTemplate == null) return null;

            var salesInvoiceTemplate = new Model.Obsolete.Obsolete26.SalesInvoiceTemplate26() { Key = new Guid("55c81ff0-2892-41fb-bff8-3fef6debba85") };
            var salesQuoteTemplate = new Model.Obsolete.Obsolete26.SalesQuoteTemplate26() { Key = new Guid("2903bbf5-6c43-4fbf-9eef-9b239b784f87") };
            var purchaseOrderTemplate = new Model.Obsolete.Obsolete26.PurchaseOrderTemplate26() { Key = new Guid("2f777546-9a69-44ec-90bf-56c38563b100") };
            var businessDetails = new Model.BusinessDetails() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.BusinessDetails)) };
            var windowFacedEnvelope = new Model.Obsolete.Obsolete32.WindowFacedEnvelope32() { Key = Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete32.WindowFacedEnvelope32)) };

            salesInvoiceTemplate.Obsolete_AmountsIncludeTax = obsoleteSalesInvoiceTemplate.AmountsIncludeTax;
            businessDetails.Address = obsoleteSalesInvoiceTemplate.BusinessContactInformation;
            businessDetails.Obsolete_BusinessIdentifier = obsoleteSalesInvoiceTemplate.BusinessIdentifier;
            businessDetails.Name = obsoleteSalesInvoiceTemplate.BusinessName;
            windowFacedEnvelope.CustomerAddressHorizontalPadding = obsoleteSalesInvoiceTemplate.CustomerAddressHorizontalPadding;
            windowFacedEnvelope.CustomerAddressVerticalPadding = obsoleteSalesInvoiceTemplate.CustomerAddressVerticalPadding;
            salesInvoiceTemplate.Obsolete_EnableAmericanExpressOnPaymentAdviceCutAway = obsoleteSalesInvoiceTemplate.EnableAmericanExpressOnPaymentAdviceCutAway;
            salesInvoiceTemplate.Obsolete_EnableDinersOnPaymentAdviceCutAway = obsoleteSalesInvoiceTemplate.EnableDinersOnPaymentAdviceCutAway;
            salesInvoiceTemplate.Obsolete_EnableMastercardOnPaymentAdviceCutAway = obsoleteSalesInvoiceTemplate.EnableMastercardOnPaymentAdviceCutAway;
            salesInvoiceTemplate.Obsolete_EnablePaymentAdviceCutAway = obsoleteSalesInvoiceTemplate.EnablePaymentAdviceCutAway;
            salesInvoiceTemplate.Obsolete_EnableVisaOnPaymentAdviceCutAway = obsoleteSalesInvoiceTemplate.EnableVisaOnPaymentAdviceCutAway;
            salesInvoiceTemplate.ReferenceNumberPrefix = obsoleteSalesInvoiceTemplate.SalesInvoiceNumberPrefix;
            salesInvoiceTemplate.Title = obsoleteSalesInvoiceTemplate.SalesInvoiceTitle;
            salesQuoteTemplate.Obsolete_Notes = obsoleteSalesInvoiceTemplate.StandardTermsOnQuote;
            businessDetails.Obsolete_BusinessIdentifier = obsoleteSalesInvoiceTemplate.TaxIdentifier;
            salesInvoiceTemplate.TermsAndPaymentAdvice = obsoleteSalesInvoiceTemplate.TermsAndPaymentAdvice;

            var list = new List<Model.Object>();

            list.Add(salesInvoiceTemplate);
            list.Add(salesQuoteTemplate);
            list.Add(purchaseOrderTemplate);
            list.Add(businessDetails);
            list.Add(windowFacedEnvelope);

            var generalSettings = objects.OfType<Model.Obsolete.Obsolete11.GeneralSettings11>().SingleOrDefault(x => x.Key == Model.Object.GetGuidByType(typeof(ManagerServer.Model.Obsolete.Obsolete11.GeneralSettings11)));
            if (generalSettings != null)
            {
                if (windowFacedEnvelope.CustomerAddressHorizontalPadding.HasValue || windowFacedEnvelope.CustomerAddressVerticalPadding.HasValue)
                {
                    generalSettings.WindowFacedEnvelope = true;
                }
                list.Add(generalSettings);
            }
            return list;
        }
    }
}

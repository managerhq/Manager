using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseQuotes
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseQuotes))]
    [Title(nameof(Strings.PurchaseQuote))]
    [Guide("Footers are customizable text sections that appear at the bottom of your purchase quotes. They provide important supplementary information to suppliers when requesting quotations.")]
    [Guide("You can create and manage multiple footer templates, each tailored for different types of purchase quotes or supplier categories. This allows you to maintain consistent, professional communication while adapting to specific procurement needs.")]
    [Header("Setting Up Footer Templates")]
    [Guide("Click the **New Footer** button to create a new footer template. Give each template a descriptive name that clearly indicates its purpose, such as *Standard RFQ Terms*, *Service Contract Requirements*, or *Equipment Purchase Terms*.")]
    [Guide("When creating a purchase quote, you can select from your saved footer templates to include the appropriate terms and conditions for that specific quotation request.")]
    [Columns]
    internal sealed class PurchaseQuoteFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.PurchaseQuoteFooter>
    {
        [Default]
        [Header("Common Footer Content")]
        [Guide("Purchase quote footers typically include important procurement information that supplements the main quote details. This helps ensure suppliers understand all requirements and terms before submitting their quotations.")]
        [Guide("Common footer content includes:")]
        [Guide("• **Delivery requirements** - Lead times, shipping methods, and delivery locations")]
        [Guide("• **Quality standards** - Specifications, certifications, or compliance requirements")]
        [Guide("• **Payment terms** - Payment schedules, methods, and any early payment discounts")]
        [Guide("• **Evaluation criteria** - How quotes will be assessed and compared")]
        [Guide("• **RFP details** - Submission deadlines, contact information, and response format requirements")]
        [Header("Managing Your Templates")]
        [Guide("Each footer template appears as a row in this list. The template name helps you quickly identify and select the right footer when creating purchase quotes.")]
        [Guide("To edit an existing footer template, click on its name. To delete a template that's no longer needed, select it and use the delete option.")]
        public string[] GetName(ManagerServer.Model.PurchaseQuoteFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesQuotes
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesQuotes))]
    [Title(nameof(Strings.SalesQuote))]
    [Guide("Sales quote footers allow you to add customizable text sections at the bottom of your *sales quotes*.")]
    [Guide("Create and manage multiple footer templates to use with different types of quotes, ensuring consistent messaging while maintaining flexibility.")]
    [Columns]
    internal sealed class SalesQuoteFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.SalesQuoteFooter>
    {
        [Default]
        [Guide("Footers are text sections that appear at the bottom of your *sales quotes*, providing important terms and conditions to potential customers.")]
        [Header("Common Uses")]
        [Guide("Footer content typically includes *quote validity periods*, pricing disclaimers, delivery terms, payment requirements, or special offers.")]
        [Guide("Different footer templates can be created for various products, services, or customer segments.")]
        [Header("Using Footer Templates")]
        [Guide("When creating a *sales quote*, you can select the appropriate footer template from your saved templates.")]
        [Guide("This ensures consistency in your quotations while allowing flexibility for different situations.")]
        [Guide("Enter a descriptive name for each footer template to easily identify its purpose, such as 'Standard Quote Terms' or 'Project-Based Services'.")]
        public string[] GetName(ManagerServer.Model.SalesQuoteFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}

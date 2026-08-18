using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.CustomFields))]
    [Guide("*Custom fields* let you add extra fields to forms and transactions to capture information specific to your business.")]
    [Guide("These fields extend Manager's standard fields, allowing you to track exactly what matters to your organization.")]
    [Header("Overview")]
    [Guide("*Custom fields* provide flexibility to adapt Manager to your unique business needs.")]
    [Guide("Whether you need to track project codes, serial numbers, warranty dates, or compliance checkboxes, custom fields make it possible.")]
    [SettingsItemScreenshot("fa-pen-field", nameof(Strings.CustomFields))]
    [Header("Getting Started")]
    [Guide("To access custom fields, navigate to the **Settings** tab, then click **Custom Fields**.")]
    [Guide("You'll see five types of custom fields available, each designed for different kinds of information.")]
    [Header("Types of Custom Fields")]
    [Guide("**Text Custom Fields** — Store text information such as reference numbers, project codes, or notes. Choose from single-line fields, multi-line paragraph fields, or dropdown lists with predefined options.")]
    [Guide("**Number Custom Fields** — Capture numerical values like quantities, measurements, or ratings. When used on line items, these fields automatically calculate totals.")]
    [Guide("**Date Custom Fields** — Record dates using a calendar picker. Perfect for tracking expiry dates, warranty periods, or any other time-sensitive information.")]
    [Guide("**Checkbox Custom Fields** — Create yes/no options for binary choices. Useful for flags like 'Priority', 'Tax Exempt', or 'Approved'.")]
    [Guide("**Multiple Value Custom Fields** — Allow selection of multiple options from a list. Ideal for categorizing with tags or attributes where items can belong to multiple categories.")]
    [Header("Where Custom Fields Appear")]
    [Guide("Custom fields can be used throughout Manager in three key ways:")]
    [Guide("• As columns in transaction lists for quick visibility")]
    [Guide("• On printed documents through *footer templates*")]
    [Guide("• In *advanced queries* for powerful reporting and analysis")]
    [Header("Showing Custom Fields as Columns")]
    [Guide("Make custom field values visible in transaction lists by clicking **Edit Columns**.")]
    [Guide("Select which custom fields to display as columns, making it easy to see important information at a glance.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.EditColumns))]
    [LinkGuide("Learn more about column customization:", typeof(NakedObjectsWithEditColumns<>))]
    [Header("Printing Custom Fields on Documents")]
    [Guide("Include custom field values on printed invoices, quotes, and other documents using **Footers**.")]
    [Guide("*Footers* use merge tags to pull custom field data into your document templates.")]
    [LinkGuide("Learn how to use footers:", typeof(Footers.Footers))]
    [Header("Reporting with Custom Fields")]
    [Guide("**Advanced Queries** unlock the full potential of custom fields for reporting.")]
    [Guide("Filter transactions by custom field values, sort by custom dates, group by categories, or create complex criteria combining multiple custom fields.")]
    [Guide("This enables you to build reports that match your exact business requirements.")]
    [LinkGuide("Learn about advanced queries:", typeof(NakedObjectsWithAdvancedQueries))]
    internal sealed class CustomFields : NakedNamespaces
    {
    }
}

using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using System.Linq;
using ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierStatements), nameof(Strings.UnpaidInvoices))]
    [Guide("The supplier statement unpaid invoices view provides a comprehensive overview of all outstanding purchase invoices for a specific supplier.")]
    [Guide("This statement helps you track which invoices remain unpaid and how long they have been outstanding, making it easier to manage your accounts payable and maintain good supplier relationships.")]
    [Header("Statement Details")]
    [Guide("The statement displays each unpaid invoice with the following information:")]
    [Guide("• **Date** - The issue date of the invoice")]
    [Guide("• **Invoice** - The invoice reference number")]
    [Guide("• **Description** - A brief description of what the invoice is for")]
    [Guide("• **Invoice Total** - The original amount of the invoice")]
    [Guide("• **Overdue** - The number of days the invoice is past its due date")]
    [Guide("• **Balance Due** - The current amount still owed on the invoice")]
    [Header("Aging Analysis")]
    [Guide("At the bottom of the statement, an aging analysis breaks down your total outstanding balance into categories based on how overdue the invoices are:")]
    [Guide("• **Current** - Invoices that are not yet due")]
    [Guide("• **1-30 days overdue** - Invoices overdue by 1 to 30 days")]
    [Guide("• **31-60 days overdue** - Invoices overdue by 31 to 60 days")]
    [Guide("• **61-90 days overdue** - Invoices overdue by 61 to 90 days")]
    [Guide("• **90+ days overdue** - Invoices overdue by more than 90 days")]
    [Guide("This aging breakdown helps you prioritize which invoices to pay first and identify any long-overdue amounts that need immediate attention.")]
    [LinkGuide("To configure the report parameters, see:", typeof(SupplierStatementsUnpaidInvoicesForm))]
    internal sealed class SupplierStatementsUnpaidInvoicesView : DefaultView<GetSupplierStatementsUnpaidInvoicesView>
    {
        protected override bool CanHaveAttachments()
        {
            return false;
        }

        protected override string GetRecipient()
        {
            return ApplicationData.Businesses.Get(Business).SingleOrDefault<Supplier>(Key)?.Email;
        }

        protected override Guid? GetCustomTheme()
        {
            return ((IHasCustomTheme)ApplicationData.Businesses.Get(Business).Single<Model.SupplierStatementsUnpaidInvoices>()).GetCustomTheme();
        }

        protected override void EditCloneButtons()
        {
            return;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [ typeof(Model.Payment) ];
        }
    }
}
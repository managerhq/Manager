using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerStatement))]
    [Guide("The Customer Statement Unpaid Invoices view shows all outstanding invoices for a customer.")]
    [Guide("It provides an aging summary of unpaid amounts for collection purposes.")]
    [LinkGuide("For more information see:", typeof(CustomerStatementsUnpaidInvoicesForm))]
    internal sealed class CustomerStatementUnpaidInvoicesView : DefaultView<GetCustomerStatementUnpaidInvoicesView>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            return ApplicationData.Businesses.Get(Business).Single<EmailTemplateForCustomerStatement>();
        }

        protected override void EditCloneButtons()
        {
            return;
        }

        protected override Guid? GetCustomTheme()
        {
            return ((IHasCustomTheme)ApplicationData.Businesses.Get(Business).Single<Model.CustomerStatementsUnpaidInvoices>()).GetCustomTheme();
        }

        protected override string GetRecipient()
        {
            return ApplicationData.Businesses.Get(Business).SingleOrDefault<Customer>(Key)?.Email;
        }

        protected override bool CanHaveAttachments()
        {
            return false;
        }

        protected override Type[] GetCopyToOptions()
        {
            return [ typeof(Model.Receipt) ];
        }
    }
}
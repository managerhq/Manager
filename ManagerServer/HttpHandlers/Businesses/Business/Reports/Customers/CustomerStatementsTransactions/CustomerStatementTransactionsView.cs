using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsTransactions;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerStatement), nameof(Strings.Transactions))]
    [Guide("The Customer Statement Transactions view shows all transactions for a customer.")]
    [Guide("It displays a detailed transaction history with running balances for the specified period.")]
    [LinkGuide("For more information see:", typeof(CustomerStatementsTransactionsForm))]
    internal sealed class CustomerStatementsTransactionsView : DefaultView<GetCustomerStatementsTransactionsView>
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
            return ((IHasCustomTheme)ApplicationData.Businesses.Get(Business).Single<Model.CustomerStatementsTransactions>()).GetCustomTheme();
        }

        protected override string GetRecipient()
        {
            return ApplicationData.Businesses.Get(Business).SingleOrDefault<Customer>(Key)?.Email;
        }

        protected override bool CanHaveAttachments()
        {
            return false;
        }
    }
}
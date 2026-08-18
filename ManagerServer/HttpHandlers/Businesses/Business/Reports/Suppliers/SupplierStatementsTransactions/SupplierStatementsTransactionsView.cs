using HttpFramework;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsTransactions;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierStatementsTransactions), nameof(Strings.View))]
    [Guide("The **Supplier Statement - Transactions** view provides a comprehensive transaction history for a selected supplier within a specified date range.")]
    [Guide("This statement displays all financial transactions between your business and the supplier, including purchases, payments, debit notes, and journal entries.")]
    [Guide("Each transaction is listed chronologically with its date, description, debit amount, credit amount, and running balance.")]
    [Guide("The statement includes an *opening balance* at the start date and calculates a *closing balance* at the end date, giving you a complete picture of your account status with the supplier.")]
    [LinkGuide("To generate this statement, see:", typeof(SupplierStatementsTransactionsForm))]
    internal sealed class SupplierStatementsTransactionsView : DefaultView<GetSupplierStatementsTransactionsView>
    {
        protected override bool CanHaveAttachments()
        {
            return false;
        }

        protected override void EditCloneButtons()
        {
            return;
        }

        protected override Guid? GetCustomTheme()
        {
            return ((IHasCustomTheme)ApplicationData.Businesses.Get(Business).Single<Model.SupplierStatementsTransactions>()).GetCustomTheme();
        }

        protected override string GetRecipient()
        {
            return ApplicationData.Businesses.Get(Business).SingleOrDefault<Supplier>(Key)?.Email;
        }
    }
}
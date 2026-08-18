using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    internal sealed class GetExpenseClaimView : GetTransactionView<Model.ExpenseClaim>
    {
        protected override TransactionView GetViewData(Model.ExpenseClaim o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.ExpenseClaim;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var employee = Database.SingleOrDefault<Model.Employee>(o.PaidBy);
            if (employee != null)
            {
                viewData.recipient.name = employee.Name;
                viewData.recipient.address = employee.Address;
                viewData.recipient.email = employee.Email;
            }

            var capitalAccount = Database.SingleOrDefault<Model.CapitalAccount>(o.PaidBy);
            if (capitalAccount != null)
            {
                viewData.recipient.name = capitalAccount.Name;
            }

            var expenseClaimsPayer = Database.SingleOrDefault<Model.ExpenseClaimsPayer>(o.PaidBy);
            if (expenseClaimsPayer != null)
            {
                viewData.recipient.name = expenseClaimsPayer.Name;
            }

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn);

            if (!string.IsNullOrWhiteSpace(o.Payee))
            {
                viewData.custom_fields.Add(new TransactionView.CustomField { label = Strings.Payee, text = o.Payee });
            }

            return viewData;
        }
    }
}

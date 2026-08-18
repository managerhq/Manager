using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("277d2af8-e377-4460-8cc9-e5ef3602eed7")]
    [Title(nameof(Strings.LatePaymentFees))]
    [Guide("The **Late Payment Fees** tab helps you track and manage penalty charges applied to customers who pay their invoices after the due date.")]
    [Guide("Late payment fees serve two important purposes: they encourage customers to pay on time and compensate your business for the cost of delayed collections.")]
    [Guide("You can set up fees as either fixed amounts or calculate them as a percentage of the overdue invoice amount.")]
    [TabScreenshot("fa-bell", nameof(Strings.LatePaymentFees))]
    [Guide("To create a new late payment fee, click the **New Late Payment Fee** button.")]
    [HeroButtonScreenshot(nameof(Strings.LatePaymentFees), nameof(Strings.NewLatePaymentFee))]
    [Guide("The **Late Payment Fees** tab displays the following information:")]
    [Columns]
    internal sealed class LatePaymentFees : NakedObjectsWithAutomaticRows<ManagerServer.Model.LatePaymentFee>
    {
        [Default]
        [WarnIfFutureDate]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("8dadbf0a-1ebd-4208-b48d-92d94b5ad44a")]
        [Guide("The date when the late payment fee was applied. This date is calculated based on the invoice due date plus any grace period you have configured.")]
        public DateTime[] GetDate(ManagerServer.Model.LatePaymentFee[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("c05440c4-7dc8-4dba-b097-d10ae799a1ce")]
        [Guide("The customer being charged the late payment fee. Click the customer name to view their full record in the **Customers** tab.")]
        public string[] GetCustomer(ManagerServer.Model.LatePaymentFee[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("fae83d70-ffc7-42b7-bb6d-d4a2db50b64c")]
        [Guide("The reference number of the overdue *sales invoice*. Click the reference number to view the original invoice and its payment history.")]
        public string[] GetSalesInvoice(ManagerServer.Model.LatePaymentFee[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.SalesInvoice>(x.SalesInvoice)?.Reference).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("63cf2d99-1b80-47c8-a875-6e9b630b0343")]
        [Guide("The amount of the late payment fee. This amount is automatically added to the customer's outstanding balance and will appear on their statement.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetAmount(ManagerServer.Model.LatePaymentFee[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, ManagerServer.Model.Currency>(0m, null)).ToArray();
        }
    }
}

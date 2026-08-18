using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{    
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Payments))]
    [Guid("2eec3cb7-eda7-4a32-82e8-162913d66ac2")]    
    [Guide("The **Payments** tab is where you record all money paid out by your business.")]
    [Guide("This includes payments to suppliers, refunds to customers, expenses, and any other outgoing funds.")]
    [Guide("Each payment decreases the balance in your bank or cash accounts.")]
    [TabScreenshot("fa-minus-square", nameof(Strings.Payments))]
    [Header("Recording Payments")]
    [Guide("To record a new payment, click the **New Payment** button.")]
    [HeroButtonScreenshot(nameof(Strings.Payments), nameof(Strings.NewPayment))]
    [LinkGuide("Learn more about payment forms:", typeof(PaymentForm))]
    [Guide("While you can manually enter payments, importing bank statements is often more efficient.")]
    [Guide("Bank imports automatically create payment transactions in bulk, saving time and reducing errors.")]
    [Guide("You can then categorize and allocate these imported transactions to the appropriate expense accounts.")]
    [LinkGuide("Learn about importing bank statements:", typeof(BankAndCashAccounts.ImportBankStatement))]
    [Header("Viewing and Managing Payments")]
    [Guide("The **Payments** tab displays your outgoing transactions with detailed information in customizable columns.")]
    [Guide("Key details include payment dates, amounts, payees, and expense allocations.")]
    [Columns]
    [Guide("Click the **Edit Columns** button to choose which columns you want to display.")]
    [LinkGuide("Learn about customizing columns:", typeof(NakedObjectsWithEditColumns<>))]
    [Guide("Each payment can have multiple lines for different expense categories or allocations.")]
    [Guide("To see all payment details broken down by line item, use the **Payments - Lines** view.")]
    [Guide("This detailed view is helpful for analyzing expenses by category or finding specific transactions.")]
    [SmallBottomButtonScreenshot(nameof(Strings.Payments)+"-"+nameof(Strings.Lines))]
    [LinkGuide("Learn about payment lines:", typeof(PaymentLines))]
    [Header("Uncategorized Payments")]
    [Guide("If any of your payments are posted to the *Suspense* account, you will see a yellow notice at the top.")]
    [Guide("The notice displays: *There is one or more uncategorized payments which can be categorized using payment rules*.")]
    [Guide("This notice commonly appears right after importing bank transactions since they are not yet categorized.")]
    [YellowNoticeScreenshot(nameof(Strings.UncategorizedPaymentsAlert))]
    [LinkGuide("When you click on the notice, you will be taken to:", typeof(UncategorizedPayments))]
    internal sealed class Payments : NakedObjectsWithAutomaticRows<Payment>
    {
        [ProtoMember(1)] public Guid? Customer;
        [ProtoMember(2)] public Guid? Supplier;

        protected override Payment[] OnGetRows(Payment[] rows)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                var accounts = userPermissions.GetBankCashAccounts().ToList();
                var filter = true;
                if (accounts.Count == 0)
                {
                    filter = false;
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>()) accounts.Add(e.Key);
                }
                if (filter) rows = rows.Where(x => (x.PaidFrom.HasValue && accounts.Contains(x.PaidFrom.Value))).ToArray();
            }

            if (Customer.HasValue) rows = rows.Where(x => x.Payee == ManagerServer.Model.Enums.PayerPayeeType.Customer && x.Customer == Customer).ToArray();
            if (Supplier.HasValue) rows = rows.Where(x => x.Payee == ManagerServer.Model.Enums.PayerPayeeType.Supplier && x.Supplier == Supplier).ToArray();

            return rows;
        }

        protected override void OnAfterHeader(Context context)
        {
            var rows = context.Get<Array>() as Payment[];

            if (rows != null && rows.Any(x => x.IsUncategorized()))
            {
                using (Div(@class: "card-header text-bg-info"))
                {
                    using (Div(@class: "flex gap-2 items-center"))
                    {
                        I(@class: "fas fa-fw fa-circle-exclamation text-neutral-400", style: "font-size: 16px");
                        using (A(href: new UncategorizedPayments() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "font-semibold"))
                        {
                            Write(Strings.UncategorizedPaymentsAlert);
                        }
                    }
                }
            }

            base.OnAfterHeader(context);
        }

        [Default, WarnIfFutureDate]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("6a0cea71-e4e7-4312-aa8d-90f6367762ea")]
        [Guide("The date when the payment was made or when funds left your account.")]
        [Guide("This date affects your financial reports and helps track when expenses were incurred.")]
        [Guide("Use the actual payment date, not the date you wrote the check or initiated the transfer.")]
        public DateTime[] GetDate(Payment[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [WarnIfFutureDate]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("acb891ac-6a85-4fb8-89bd-bb27342bc83d")]
        [Guide("The date when the payment appeared on your bank statement, confirming funds have been withdrawn.")]
        [Guide("Cleared payments are reconciled transactions that match your bank records.")]
        [Guide("Payments without a cleared date are pending and help you track outstanding checks and transfers.")]
        public DateTime?[] GetCleared(Payment[] rows)
        {
            return rows.Select(x => x.GetClearDate()).ToArray();
        }

        [MinWidth, PaddedSorting]
        [Guid("b294fa10-4c81-4a25-8fcd-15933f518c33")]
        [Guide("A unique reference number or identifier for this payment.")]
        [Guide("This could be a check number, wire transfer reference, or transaction ID.")]
        [Guide("References help match payments to bank statements and resolve payment inquiries.")]
        public string[] GetReference(Payment[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("9485a2dd-2c2b-40e4-8d71-5f2728ddfb53")]
        [Guide("The bank account, cash account, or credit card used to make this payment.")]
        [Guide("Selecting the correct account ensures your account balances remain accurate.")]
        [Guide("If you have multiple accounts, this helps track which funds were used.")]
        public string[] GetPaidFrom(Payment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.PaidFrom)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("e044bbfe-3005-4008-94d3-a562cb746a31")]
        [Guide("A brief description explaining what this payment was for.")]
        [Guide("Good descriptions help you remember transaction details months or years later.")]
        [Guide("Include invoice numbers, purchase details, or other relevant information.")]
        public string[] GetDescription(Payment[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default]
        [Guid("4761f56b-b6a5-4628-b39c-14892c13c46b")]
        [Guide("The person or business who received this payment.")]
        [Guide("This could be a supplier you're paying, a customer receiving a refund, or another payee.")]
        [Guide("Accurate payee information helps track spending by vendor and generate supplier reports.")]
        public string[] GetPayee(Payment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.Contact).ToArray();
        }

        [Guid("0b761e5a-2adf-4f55-9826-735de8ac1042")]
        [Guide("The expense or asset accounts that categorize what this payment was for.")]
        [Guide("Proper categorization ensures accurate financial statements and expense tracking.")]
        [Guide("Multiple accounts indicate the payment was split between different expense categories.")]
        public string[] GetAccounts(Payment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => !x.IsBalancing).Select(x => x.Account).Distinct())).ToArray();
        }

        [Guid("af743efe-0bca-41a3-a82e-9b1dbdff862e")]
        [Guide("Shows which projects or jobs this payment relates to when using *project tracking*.")]
        [Guide("Project allocation helps track costs and profitability by project.")]
        [Guide("Multiple projects indicate the payment was split between different jobs.")]
        public string[] GetProject(Payment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Project != null).Select(x => x.Project.Name).Distinct())).ToArray();
        }

        [Sum]
        [Default, Bold, Right, WhitespaceNoWrap]
        [Guid("6d0233bd-5237-465d-8bdc-29dafd2052ea")]
        [Guide("The total amount of money paid out in this transaction.")]
        [Guide("For foreign currency payments, both the foreign amount and *base currency* equivalent are shown.")]
        [Guide("This amount will decrease your bank account balance and increase your expenses or assets.")]
        public Tuple<decimal, Currency>[] GetAmount(Payment[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!Customer.HasValue && !Supplier.HasValue)
            {
                using (A(href: new PaymentsFindAndRecode() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.FindAndRecode);
                using (A(href: new PaymentLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.Payment + " - " + Strings.Lines);
            }
            base.OnFooterEndSection(context);
        }
    }
}

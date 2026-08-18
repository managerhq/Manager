using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payments
{
    [ProtoContract]
    [Title(nameof(Strings.UncategorizedPayments))]
    [Guide("The Uncategorized Payments screen lists all payments that have not been assigned to specific accounts or expense categories.")]
    [Guide("Each payment can be edited directly from this screen to assign the appropriate category, adjust the date, amount, or other details.")]
    [Guide("You can use search and filter options to quickly find specific transactions or sort them by date, amount, or description.")]
    [Header("Accessing Uncategorized Payments")]
    [Guide("To access the Uncategorized Payments screen, go to the **Payments** tab.")]
    [TabScreenshot("fa-minus-square", nameof(Strings.Payments))]
    [Guide("Then click the **Uncategorized Payments** button.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.UncategorizedPayments))]
    [Header("Creating Payment Rules")]
    [Guide("Payment rules can be created directly from the Uncategorized Payments screen. When an uncategorized payment cannot be matched to any existing payment rule, you will see a **New Payment Rule** button next to the description.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.NewPaymentRule))]
    [Guide("Clicking this button takes you to a pre-filled form where you can configure a new payment rule based on the uncategorized transaction.")]
    [LinkGuide("Learn more about payment rules:", typeof(Settings.BankRules.PaymentRules.PaymentRules))]
    [Header("Batch Categorization")]
    [Guide("You can batch categorize payments where a payment rule has been matched.")]
    [Guide("Select the payments you wish to categorize in bulk.")]
    [Guide("Click the **Batch Update** button at the bottom of the screen.")]
    [Guide("The selected payments will be categorized and removed from the Uncategorized Payments screen.")]
    [Guide("If you categorize transactions by mistake, you can undo the operation using the History screen.")]
    [LinkGuide("Learn more about history:", typeof(History))]
    internal sealed class UncategorizedPayments : Table<UncategorizedPayments.Row>
    {
        [ProtoMember(1)] public Guid? BankAccount;

        protected override byte[] GetCheckbox(Row o)
        {
            return o.Checkbox;
        }

        protected override BusinessTemplate GetEdit(Row o, string referrer)
        {
            return new PaymentForm() { Business = Business, Key = o.Key, Referrer = referrer };
        }

        protected override BusinessTemplate GetView(Row o, string referrer)
        {
            return new PaymentView() { Business = Business, Key = o.Key, Referrer = referrer };
        }

        protected override Row[] GetObjects()
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var paymentRules = database.OfType<ManagerServer.Model.PaymentRule>().ToArray();
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();

            var rows = ApplicationData.Businesses.Get(Business).OfType<Payment>();

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

            rows = rows.Where(x => x.IsUncategorized()).ToArray();
            if (BankAccount.HasValue) rows = rows.Where(x => x.PaidFrom == BankAccount).ToArray();

            var output = new List<Row>();
            foreach (var e in rows)
            {
                var decimals = baseCurrency.GetDecimalPlaces();
                var bankAccount = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.BankOrCashAccount>(e.PaidFrom);
                if (bankAccount != null)
                {
                    var foreignCurrency = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.ForeignCurrency>(bankAccount.Currency);
                    if (foreignCurrency != null) decimals = foreignCurrency.GetDecimalPlaces();
                }

                var paymentTransaction = e.GetGeneralLedgerTransactions(database)?.FirstOrDefault(x => x.IsBalancing);

                var paymentRule = paymentRules.Where(x => x.IsMatch(paymentTransaction.BankAccount?.Key, e.Description, paymentTransaction.AccountAmount * -1m)).OrderByDescending(x => x.GetRuleLength()).FirstOrDefault();
                if (paymentRule?.Lines != null && paymentRule.Lines.Length > 0)
                {
                    var payment = (Payment)e.DeepClone();
                    payment.Payee = paymentRule.Payee;
                    payment.Customer = paymentRule.Customer;
                    payment.Supplier = paymentRule.Supplier;
                    payment.Contact = paymentRule.OtherContact;
                    if (paymentRule.QuantityColumn) payment.QuantityColumn = paymentRule.QuantityColumn;
                    payment.HasLineDescription = paymentRule.DescriptionColumn;
                    payment.Lines ??= [new Payment.Line()];

                    if (paymentRule.Lines.Length == 1)
                    {
                        Copy(paymentRule.Lines[0], payment.Lines[0]);
                        payment.Lines[0].Amount = -paymentTransaction.AccountAmount;
                        if (e.Lines != null && e.Lines.Length == 1 && e.Lines[0].Qty.HasValue) payment.Lines[0].Qty = e.Lines[0].Qty.Value;
                    }
                    else
                    {
                        var lines = new List<ManagerServer.Model.Payment.Line>();

                        foreach (var e2 in paymentRule.Lines.Where(x => x.Amount == ManagerServer.Model.Enums.DiscountType.ExactAmount))
                        {
                            var line = new ManagerServer.Model.Payment.Line();
                            Copy(e2, line);
                            line.Amount = e2.ExactAmount;
                            lines.Add(line);
                        }

                        var reminder = payment.Lines[0].Amount - lines.Sum(x => x.Amount);

                        if (reminder != 0m)
                        {
                            foreach (var e2 in paymentRule.Lines.Where(x => x.Amount == ManagerServer.Model.Enums.DiscountType.Percentage))
                            {
                                var line = new ManagerServer.Model.Payment.Line();
                                Copy(e2, line);
                                line.Amount = Math.Round(reminder / 100 * e2.Percentage, decimals, MidpointRounding.AwayFromZero);
                                lines.Add(line);
                            }
                        }

                        reminder = payment.Lines[0].Amount - lines.Sum(x => x.Amount);
                        if (reminder != 0m && lines.Any() && paymentRule.Lines.Where(x => x.Amount == ManagerServer.Model.Enums.DiscountType.Percentage).Sum(x => x.Percentage) == 100m)
                        {
                            lines.Last().Amount += reminder;
                        }
                        else if (reminder != 0m)
                        {
                            lines.Add(new ManagerServer.Model.Payment.Line()
                            {
                                Amount = reminder
                            });
                        }
                        payment.Lines = lines.ToArray();
                    }

                    using (var ms = new MemoryStream())
                    {
                        var newPaymentTransaction = payment.CreateGeneralLedgerTransactions(database)?.FirstOrDefault(x => x.IsBalancing);

                        ProtoBuf.Serializer.Serialize<Tuple<Guid, Payment>>(ms, new Tuple<Guid, Payment>(e.Key, payment));
                        output.Add(new Row()
                        {
                            Key = e.Key,
                            Checkbox = ms.ToArray(),
                            Date = e.Date,
                            Description = new StringWithLinkButton(e.Description, new LinkButton(new Settings.BankRules.PaymentRules.PaymentRuleForm() { Business = Business, Referrer = referrer, Key = paymentRule.Key }.ToUrl(), Strings.EditBankRule)),
                            Account = new Delta(Strings.Suspense, string.Join(", ", newPaymentTransaction?.ContraTransactions.Select(x => x.Account).Distinct())),
                            Payee = new Delta(null, newPaymentTransaction.Contact),
                            Amount = paymentTransaction.GetReversedTransactionAmountWithCurrency(),
                            BankOrCashAccount = paymentTransaction.BankAccount?.Name
                        });
                    }
                }
                else
                {
                    output.Add(new Row()
                    {
                        Key = e.Key,
                        Checkbox = null,
                        Date = e.Date,
                        Description = new StringWithLinkButton(e.Description, new LinkButton(new Settings.BankRules.PaymentRules.PaymentRuleForm() { Business = Business, Referrer = referrer, BankAccount = e.PaidFrom, Description = e.Description }.ToUrl(), Strings.NewPaymentRule)),
                        Account = new Delta(Strings.Suspense, Strings.Suspense),
                        Amount = paymentTransaction.GetReversedTransactionAmountWithCurrency(),
                        BankOrCashAccount = paymentTransaction.BankAccount?.Name
                    });
                }
            }
            return output.ToArray();
        }

        public sealed class Row
        {
            public Guid Key;
            public byte[] Checkbox;

            [Center, WhitespaceNoWrap, MinWidth] public DateTime Date { get; set; }
            public Delta Account { get; set; }
            [HideColumnIfAllEmpty] public Delta Payee { get; set; }
            public string BankOrCashAccount { get; set; }
            public StringWithLinkButton Description { get; set; }
            [Bold, Sum, TabularNums, Right] public Tuple<decimal, Currency> Amount { get; set; }
        }

        protected override void OnCustomCheckbox(byte[][] values)
        {
            var list = new List<Payment>();
            foreach (var e in values)
            {
                using (var ms = new MemoryStream(e))
                {
                    var e2 = ProtoBuf.Serializer.Deserialize<Tuple<Guid, ManagerServer.Model.Payment>>(ms);
                    e2.Item2.Key = e2.Item1;
                    list.Add(e2.Item2);
                }
            }
            ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
        }
    }
}
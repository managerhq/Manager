using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.UncategorizedReceipts))]
    [Guide("The **Uncategorized Receipts** screen displays all receipts that have not yet been allocated to specific accounts.")]
    [Guide("These receipts are typically imported from bank statements or entered manually without proper account allocation.")]
    [Header("Automatic Categorization")]
    [Guide("The system suggests automatic categorization for uncategorized receipts based on your configured *bank rules*.")]
    [Guide("When a receipt matches a bank rule, the suggested account allocation appears in green text, allowing you to quickly review and apply the suggestion.")]
    [Header("Bulk Operations")]
    [Guide("You can select multiple receipts using the checkboxes and click **Batch Update** to apply the suggested categorizations to all selected receipts at once.")]
    [Guide("This feature saves time when processing multiple similar transactions that match your bank rules.")]
    [Columns]
    internal sealed class UncategorizedReceipts : Table<UncategorizedReceipts.Row>
    {
        [ProtoMember(1)] public Guid? BankAccount;

        protected override Row[] GetObjects()
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var receiptRules = database.OfType<ManagerServer.Model.ReceiptRule>();
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();

            var receipts = ApplicationData.Businesses.Get(Business).OfType<Receipt>();

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
                if (filter) receipts = receipts.Where(x => (x.ReceivedIn.HasValue && accounts.Contains(x.ReceivedIn.Value))).ToArray();
            }

            receipts = receipts.Where(x => x.IsUncategorized()).ToArray();
            if (BankAccount.HasValue) receipts = receipts.Where(x => x.ReceivedIn == BankAccount).ToArray();

            var output = new List<Row>();
            foreach (var e in receipts)
            {
                var decimals = baseCurrency.GetDecimalPlaces();
                var bankAccount = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.BankOrCashAccount>(e.ReceivedIn);
                if (bankAccount != null)
                {
                    var foreignCurrency = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.ForeignCurrency>(bankAccount.Currency);
                    if (foreignCurrency != null) decimals = foreignCurrency.GetDecimalPlaces();
                }

                var receiptTransaction = e.GetGeneralLedgerTransactions(database)?.FirstOrDefault(x => x.IsBalancing);

                var receiptRule = receiptRules.Where(x => x.IsMatch(receiptTransaction.BankAccount?.Key, e.Description, receiptTransaction.AccountAmount)).OrderByDescending(x => x.GetRuleLength()).FirstOrDefault();
                if (receiptRule != null)
                {
                    var receipt = (Receipt)e.DeepClone();
                    receipt.PaidBy = receiptRule.PaidBy;
                    receipt.Customer = receiptRule.Customer;
                    receipt.Supplier = receiptRule.Supplier;
                    receipt.Contact = receiptRule.OtherContact;
                    if (receiptRule.QuantityColumn) receipt.QuantityColumn = receiptRule.QuantityColumn;
                    receipt.HasLineDescription = receiptRule.DescriptionColumn;
                    receipt.Lines ??= [new Receipt.Line()];

                    if (receiptRule.Lines.Length == 1)
                    {
                        Copy(receiptRule.Lines[0], receipt.Lines[0]);
                        receipt.Lines[0].Amount = receiptTransaction.AccountAmount;
                        if (e.Lines != null && e.Lines.Length == 1 && e.Lines[0].Qty.HasValue) receipt.Lines[0].Qty = e.Lines[0].Qty.Value;
                    }
                    else
                    {
                        var lines = new List<ManagerServer.Model.Receipt.Line>();

                        foreach (var e2 in receiptRule.Lines.Where(x => x.Amount == ManagerServer.Model.Enums.DiscountType.ExactAmount))
                        {
                            var line = new ManagerServer.Model.Receipt.Line();
                            Copy(e2, line);
                            line.Amount = e2.ExactAmount;
                            lines.Add(line);
                        }

                        var reminder = receipt.Lines[0].Amount - lines.Sum(x => x.Amount);

                        if (reminder != 0m)
                        {
                            foreach (var e2 in receiptRule.Lines.Where(x => x.Amount == ManagerServer.Model.Enums.DiscountType.Percentage))
                            {
                                var line = new ManagerServer.Model.Receipt.Line();
                                Copy(e2, line);
                                line.Amount = Math.Round(reminder / 100 * e2.Percentage, decimals, MidpointRounding.AwayFromZero);
                                lines.Add(line);
                            }
                        }

                        reminder = receipt.Lines[0].Amount - lines.Sum(x => x.Amount);
                        if (reminder != 0m && lines.Any() && receiptRule.Lines.Where(x => x.Amount == ManagerServer.Model.Enums.DiscountType.Percentage).Sum(x => x.Percentage) == 100m)
                        {
                            lines.Last().Amount += reminder;
                        }
                        else if (reminder != 0m)
                        {
                            lines.Add(new ManagerServer.Model.Receipt.Line()
                            {
                                Amount = reminder
                            });
                        }
                        receipt.Lines = lines.ToArray();
                    }

                    using (var ms = new MemoryStream())
                    {
                        var newReceiptTransaction = receipt.CreateGeneralLedgerTransactions(database)?.FirstOrDefault(x => x.IsBalancing);

                        ProtoBuf.Serializer.Serialize<Tuple<Guid, Receipt>>(ms, new Tuple<Guid, Receipt>(e.Key, receipt));
                        output.Add(new Row()
                        {
                            Key = e.Key,
                            Checkbox = ms.ToArray(),
                            Date = e.Date,
                            Description = new StringWithLinkButton(e.Description, new LinkButton(new Settings.BankRules.ReceiptRules.ReceiptRuleForm() { Business = Business, Referrer = referrer, Key = receiptRule.Key }.ToUrl(), Strings.EditBankRule)),
                            Account = new Delta(Strings.Suspense, string.Join(", ", newReceiptTransaction?.ContraTransactions.Select(x => x.Account).Distinct())),
                            Payer = new Delta(null, newReceiptTransaction.Contact),
                            Amount = receiptTransaction.GetTransactionAmountWithCurrency(),
                            BankOrCashAccount = receiptTransaction.BankAccount?.Name
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
                        Description = new StringWithLinkButton(e.Description, new LinkButton(new Settings.BankRules.ReceiptRules.ReceiptRuleForm() { Business = Business, Referrer = referrer, BankAccount = e.ReceivedIn, Description = e.Description }.ToUrl(), Strings.NewReceiptRule)),
                        Account = new Delta(Strings.Suspense, Strings.Suspense),
                        Amount = receiptTransaction.GetTransactionAmountWithCurrency(),
                        BankOrCashAccount = receiptTransaction.BankAccount?.Name
                    });
                }
            }
            return output.ToArray();
        }

        protected override byte[] GetCheckbox(Row o)
        {
            return o.Checkbox;
        }

        protected override BusinessTemplate GetEdit(Row o, string referrer)
        {
            return new ReceiptForm() { Business = Business, Key = o.Key, Referrer = referrer };
        }

        protected override BusinessTemplate GetView(Row o, string referrer)
        {
            return new ReceiptView() { Business = Business, Key = o.Key, Referrer = referrer };
        }

        public sealed class Row
        {
            public Guid Key;
            public byte[] Checkbox;

            [MinWidth, WhitespaceNoWrap, Center] public DateTime Date { get; set; }
            public Delta Account { get; set; }
            [HideColumnIfAllEmpty] public Delta Payer { get; set; }
            public string BankOrCashAccount { get; set; }
            public StringWithLinkButton Description { get; set; }
            [Bold, Sum, TabularNums, WhitespaceNoWrap, Right] public Tuple<decimal, Currency> Amount { get; set; }
        }

        protected override void OnCustomCheckbox(byte[][] values)
        {
            var list = new List<Receipt>();
            foreach (var e in values)
            {
                using (var ms = new MemoryStream(e))
                {
                    var e2 = ProtoBuf.Serializer.Deserialize<Tuple<Guid, ManagerServer.Model.Receipt>>(ms);
                    e2.Item2.Key = e2.Item1;
                    list.Add(e2.Item2);
                }
            }
            ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
        }
    }
}

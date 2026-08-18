using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.NewInterAccountTransfer))]
    [Guide("The **New Inter Account Transfer** screen helps you convert separate payment and receipt transactions into proper inter account transfers.")]
    [Guide("This feature automatically finds matching payments and receipts that represent money moving between your accounts.")]
    [Header("When to Use This Feature")]
    [Guide("This screen is particularly useful when importing bank transactions that automatically create separate payments and receipts for transfers between accounts.")]
    [Guide("Instead of having two separate transactions, you can convert them into a single *inter account transfer* for cleaner bookkeeping.")]
    [Header("How to Create Inter Account Transfers")]
    [Guide("When recording a payment or receipt that represents a transfer between accounts, categorize it to the **InterAccountTransfers** account.")]
    [Guide("Select the bank account where the money came from (for payments) or was deposited to (for receipts).")]
    [Guide("Once you have matching payment and receipt transactions of the same amount, this screen will display the pairs that can be converted to inter account transfers.")]
    [Header("Accessing This Screen")]
    [Guide("When matching transactions are available, a yellow notice appears at the top of the **Inter Account Transfers** tab.")]
    [Guide("Click the yellow notice to access this screen and convert your matching transactions.")]
    internal sealed class NewInterAccountTransfers : NakedObjectsWithCustomFields<NewInterAccountTransfers.Item>
    {
        public Item[] GetItems()
        {
            var list = new List<Item>();

            var database = ApplicationData.Businesses.Get(Business);

            var payments = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in database.OfType<ManagerServer.Model.Payment>().Where(x => x.Lines != null).Where(x => x.Lines.Length == 1).SelectMany(x => x.GetGeneralLedgerTransactions(database).Where(x => x.InterAccountTransferPair != null)))
            {
                payments.Add(e);
            }

            var receipts = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            foreach (var e in database.OfType<ManagerServer.Model.Receipt>().Where(x => x.Lines != null).Where(x => x.Lines.Length == 1).SelectMany(x => x.GetGeneralLedgerTransactions(database).Where(x => x.InterAccountTransferPair != null)))
            {
                receipts.Add(e);
            }
            receipts = receipts.OrderBy(x => x.Date).ToList();

            foreach (var e in payments.ToArray())
            {
                var receipt = receipts.Where(x => x.BankAccount == e.InterAccountTransferAccount && x.BaseAmount == e.BaseAmount * -1m).OrderBy(x => Math.Abs((x.Date - e.Date).Ticks)).FirstOrDefault();
                if (receipt != null)
                {
                    receipts.Remove(receipt);

                    list.Add(new Item()
                    {
                        Payment = e.Payment,
                        Receipt = receipt.Receipt,
                    });
                }
            }

            return list.ToArray();
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(Item[] rows)
        {
            return rows.Select(x => new Tuple<string, byte[]>(nameof(NewInterAccountTransfers), x.Payment.Key.ToByteArray().Concat(x.Receipt.Key.ToByteArray()).ToArray())).ToArray();
        }

        public override int GetContextCount()
        {
            return GetItems().Length;
        }

        protected override void InnerGet4(Context context)
        {
            context.Set<Array>(GetItems());
            context.Set(new BatchOperation() { Name = Strings.NewInterAccountTransfer });
            base.InnerGet4(context);
        }

        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-edit")]
        public BusinessTemplate[] GetPaymentEdit(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Payments.PaymentForm() { Business = Business, Key = x.Payment.Key, Referrer = referrer }).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-eye")]
        public BusinessTemplate[] GetPaymentView(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Payments.PaymentView() { Business = Business, Key = x.Payment.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("b6ac4225-c149-40bb-834f-e5198267b20a")]
        [Name(nameof(Strings.Date))]
        public DateTime[] GetPaymentDate(Item[] rows)
        {
            return rows.Select(x => x.Payment.Date).ToArray();
        }

        [Default]
        [Guid("3c0b561f-d533-4594-985a-af480f94524f")]
        public string[] GetPaidFrom(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.Payment.PaidFrom)?.GetCodeAndName()).ToArray();
        }

        [Default, Right, Bold, MinWidth]
        [Guid("e08f4b76-7408-4f7c-81c6-7b1b7a135b51")]
        [Name(nameof(Strings.Amount))]
        public Tuple<decimal, Currency>[] GetPaymentAmount(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Payment.GetGeneralLedgerTransactions(database).Single(x => x.IsBalancing).GetReversedTransactionAmountWithCurrency()).ToArray();
        }

        [Default]
        [Name("")]
        [Center]
        public string[] GetDivider(Item[] rows)
        {
            var icon = @"<i class=""fas fa-arrow-right text-base opacity-50"" />";
            return rows.Select(x => icon).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-edit")]
        public BusinessTemplate[] GetReceiptEdit(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Receipts.ReceiptForm() { Business = Business, Key = x.Receipt.Key, Referrer = referrer }).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-eye")]
        public BusinessTemplate[] GetReceiptView(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Receipts.ReceiptView() { Business = Business, Key = x.Receipt.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Name(nameof(Strings.Date))]
        [Guid("b38295e2-f886-4ef7-8058-33c17666663a")]
        public DateTime[] GetReceiptDate(Item[] rows)
        {
            return rows.Select(x => x.Receipt.Date).ToArray();
        }

        [Default]
        [Guid("137850ee-e8eb-4087-88b2-771ed4141a3c")]
        public string[] GetReceivedIn(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<BankOrCashAccount>(x.Receipt.ReceivedIn)?.GetCodeAndName()).ToArray();
        }

        [Default, Right, Bold, MinWidth]
        [Name(nameof(Strings.Amount))]
        [Guid("5a42bdf8-d4d2-4460-b76c-3d4c49d92f2f")]
        public Tuple<decimal, Currency>[] GetReceiptAmount(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Receipt.GetGeneralLedgerTransactions(database).Single(x => x.IsBalancing).GetTransactionAmountWithCurrency()).ToArray();
        }

        [ProtoContract]
        public sealed class Item
        {
            [ProtoMember(1)] public Payment Payment;
            [ProtoMember(2)] public Receipt Receipt;
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey(nameof(NewInterAccountTransfers)))
                {
                    var item = form[nameof(NewInterAccountTransfers)].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var keys = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        var database = ApplicationData.Businesses.Get(Business);

                        var items = GetItems();

                        var actions = new List<ManagerServer.ApplicationData.Action>();

                        foreach (var key in keys)
                        {
                            var paymentKey = new Guid(key.Take(16).ToArray());
                            var receiptKey = new Guid(key.Skip(16).ToArray());

                            var pair = GetItems().SingleOrDefault(x => x.Payment.Key == paymentKey && x.Receipt.Key == receiptKey);
                            if (pair != null)
                            {
                                var date = (new DateTime[] { pair.Payment.Date, pair.Receipt.Date }).Min();

                                var creditClearStatus = ManagerServer.Model.Enums.BankAccountClearStatus.OnTheSameDate;
                                DateTime? creditClearDate = default;

                                if (date != pair.Payment.GetClearDate())
                                {
                                    creditClearStatus = ManagerServer.Model.Enums.BankAccountClearStatus.OnALaterDate;
                                    creditClearDate = pair.Payment.GetClearDate();
                                }

                                var debitClearStatus = ManagerServer.Model.Enums.BankAccountClearStatus.OnTheSameDate;
                                DateTime? debitClearDate = default;

                                if (date != pair.Receipt.GetClearDate())
                                {
                                    debitClearStatus = ManagerServer.Model.Enums.BankAccountClearStatus.OnALaterDate;
                                    debitClearDate = pair.Receipt.GetClearDate();
                                }

                                var interAccountTransfer = new InterAccountTransfer()
                                {
                                    Date = date,
                                    Description = pair.Payment.Description+" "+pair.Receipt.Description,
                                    PaidFrom = pair.Payment.PaidFrom,
                                    ReceivedIn = pair.Receipt.ReceivedIn,
                                    CreditClearDate = creditClearDate,
                                    DebitClearDate = debitClearDate,
                                    CreditClearStatus = creditClearStatus,
                                    DebitClearStatus = debitClearStatus,
                                    CreditAmount = pair.Payment.Lines[0].Amount,
                                    DebitAmount = pair.Receipt.Lines[0].Amount
                                };

                                actions.Add(new ManagerServer.ApplicationData.CreateOrUpdateAction(interAccountTransfer));
                                actions.Add(new ManagerServer.ApplicationData.DeleteAction(pair.Receipt.Key));
                                actions.Add(new ManagerServer.ApplicationData.DeleteAction(pair.Payment.Key));
                            }
                        }

                        if (actions.Any())
                        {
                            ApplicationData.Businesses.Process(Business, actions.ToArray(), GetUserName());
                        }

                        Response.Redirect(this.ToUrl());
                        return;
                    }
                }
            }
            await base.InnerPost();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [Guid("a79da0d5-de39-4098-b24f-ba4762c69674")]
    [Title(nameof(Strings.EarlyPaymentDiscounts))]
    [Guide("This screen displays sales invoices where customers have earned early payment discounts by paying within the specified discount period.")]
    [Guide("When customers pay invoices early enough to qualify for the discount, they appear in this list.")]
    [Guide("To issue credit notes for the earned discounts, select the relevant invoices and click the **New Credit Note** button.")]
    [Guide("The table below shows qualifying invoices with their discount amounts:")]
    [Columns]
    internal sealed class PendingEarlyPaymentDiscounts : NakedObjectsWithCustomFields<PendingEarlyPaymentDiscounts.Item>
    {
        [ProtoContract]
        public sealed class Item
        {
            [ProtoMember(1)] public Guid? Customer;
            [ProtoMember(2)] public Guid Invoice;
            [ProtoMember(3)] public decimal InvoiceTotal;
            [ProtoMember(4)] public decimal BalanceDue;
            [ProtoMember(5)] public decimal EarlyPaymentDiscount;
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(Item[] rows)
        {
            var list = new List<Tuple<string, byte[]>>();
            foreach (var e in rows)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, e);
                    list.Add(new Tuple<string, byte[]>("PendingEarlyPaymentDiscounts", ms.ToArray()));
                }
            }
            return list.ToArray();
        }

        public override BusinessTemplate[] GetEdit(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesInvoices.SalesInvoiceForm() { Business = Business, Key = x.Invoice, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesInvoices.SalesInvoiceView() { Business = Business, Key = x.Invoice, Referrer = referrer }).ToArray();
        }

        [Default]
        [Guid("70b5f1c7-2cee-4065-b907-783991845e7a")]
        [Guide("The *Customer* column shows the customer who earned the early payment discount.")]
        public string[] GetCustomer(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Customer>(x.Customer)?.Name).ToArray();
        }

        [Default]
        [Guid("1b56358b-1b8f-4d62-910a-4f1ba003d882")]
        [Guide("The *Sales Invoice* column displays the invoice reference that was paid early to qualify for the discount.")]
        public string[] GetSalesInvoice(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.SalesInvoice>(x.Invoice)?.GetName()).ToArray();
        }

        [Default]
        [Guid("457c595a-55d8-414d-ad5a-ada86b6a8476")]
        [Guide("The *Balance Due* column shows the current outstanding balance on the invoice.")]
        public decimal[] GetBalanceDue(Item[] rows)
        {
            return rows.Select(x => x.BalanceDue).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("7d2bb9a0-71e2-4d22-92bb-6c028e9a6e3c")]
        [Guide("The *Early Payment Discount* column displays the discount amount earned by the customer for paying within the discount period.")]
        public decimal[] GetEarlyPaymentDiscount(Item[] rows)
        {
            return rows.Select(x => x.EarlyPaymentDiscount).ToArray();
        }

        public override int GetContextCount()
        {
            return GetItems().Length;
        }

        protected override void InnerGet4(Context context)
        {
            context.Set<Array>(GetItems());
            context.Set(new BatchOperation() { Name = Strings.NewCreditNote });

            base.InnerGet4(context);
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey("PendingEarlyPaymentDiscounts"))
                {
                    var pendingEarlyPaymentDiscounts = form["PendingEarlyPaymentDiscounts"].ToString();
                    if (!string.IsNullOrWhiteSpace(pendingEarlyPaymentDiscounts))
                    {
                        var items = pendingEarlyPaymentDiscounts.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        SetCulture(Business);

                        var list = new List<ManagerServer.Model.Object>();

                        var reference = GetNextCreditNoteReferenceNumber();
                        var salesInvoicesByKey = ApplicationData.Businesses.Get(Business).OfType<SalesInvoice>().ToDictionary(x => x.Key);
                        var taxCodes = ApplicationData.Businesses.Get(Business).OfType<TaxCode>().ToDictionary(x => x.Key);
                        var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

                        foreach (var e in items)
                        {
                            using (var ms = new System.IO.MemoryStream(e))
                            {
                                var item = ProtoBuf.Serializer.Deserialize<Item>(ms);

                                var salesInvoice = salesInvoicesByKey[item.Invoice];
                                var creditNote = new CreditNote() { Key = Guid.CreateVersion7() };

                                Copy(salesInvoice, creditNote);
                                creditNote.Reference = reference.ToString();
                                creditNote.IssueDate = DateTime.Today;
                                creditNote.AmountsIncludeTax = true;
                                creditNote.SalesInvoice = salesInvoice.Key;
                                creditNote.Type = ManagerServer.Model.Enums.CreditNoteType.EarlyPaymentDiscount;
                                creditNote.Description = Strings.EarlyPaymentDiscount;

                                var currency = ApplicationData.Businesses.Get(Business).SingleOrDefault<ForeignCurrency>(salesInvoice.Customer) as Currency ?? baseCurrency;

                                foreach (var e2 in creditNote.Lines)
                                {
                                    var lineAmount = e2.SalesUnitPrice;
                                    try { if (e2.Qty.HasValue) lineAmount *= e2.Qty.Value; } catch (OverflowException) { }
                                    if (lineAmount != 0m && e2.DiscountPercentage != 0m) lineAmount = lineAmount / 100m * (100m - e2.DiscountPercentage);
                                    if (e2.DiscountAmount != 0m) lineAmount -= e2.DiscountAmount;
                                    lineAmount = currency.Round(lineAmount);
                                    if (!salesInvoice.AmountsIncludeTax && e2.TaxCode.HasValue && taxCodes.ContainsKey(e2.TaxCode.Value)) lineAmount += taxCodes[e2.TaxCode.Value].CalculateTaxAmounts(lineAmount, currency.GetDecimalPlaces(), false).Sum(x => x.Amount);

                                    e2.CustomFields = null;
                                    e2.DiscountPercentage = 0m;
                                    e2.DiscountAmount = 0m;
                                    e2.Qty = null;

                                    e2.SalesUnitPrice = lineAmount;
                                }

                                var creditNoteTotal = creditNote.Lines.Sum(x => x.SalesUnitPrice);
                                var creditNoteRatio = item.EarlyPaymentDiscount / creditNoteTotal;

                                foreach (var e2 in creditNote.Lines)
                                {
                                    e2.SalesUnitPrice = currency.Round(e2.SalesUnitPrice * creditNoteRatio);
                                }

                                ApplicationData.Businesses.Process(Business, creditNote, GetUserName());

                                reference++;
                            }
                        }

                        ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());

                        Response.Redirect(this.ToUrl());
                        return;
                    }
                }
            }
            await base.InnerPost();
        }

        public long GetNextCreditNoteReferenceNumber()
        {
            var references = ApplicationData.Businesses.Get(Business).OfType<CreditNote>().Where(x => !string.IsNullOrWhiteSpace(x.Reference)).Select(x => x.Reference).ToArray();
            long reference = 1;
            foreach (var e in references)
            {
                if (string.IsNullOrWhiteSpace(e)) continue;
                var s = string.Join("", e.ToCharArray().Where(x => char.IsDigit(x)));
                if (string.IsNullOrWhiteSpace(s)) continue;
                long i = 0;
                if (long.TryParse(s, out i))
                {
                    if (i >= reference) reference = i + 1;
                }
            }
            return reference;
        }

        internal Item[] GetItems()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var salesInvoices = database.OfType<ManagerServer.Model.SalesInvoice>().ToArray();
            var salesInvoicesByKey = salesInvoices.ToDictionary(x => x.Key);
            var salesInvoiceKeys = new HashSet<Guid>(salesInvoices.Select(x => x.Key));
            salesInvoices = salesInvoices.OrderByDescending(x => x.IssueDate).ThenByDescending(x => x.Reference).ToArray();

            var customers = database.OfType<ManagerServer.Model.Customer>().ToDictionary(x => x.Key, x => x.NameWithCode);
            var customerCurrencies = database.OfType<ManagerServer.Model.Customer>().ToDictionary(x => x.Key, x => x.Currency);

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices();
            var salesInvoiceTransactions = generalLedger.Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.SalesInvoice != null).ToArray();
            var salesInvoiceAmounts = salesInvoiceTransactions.Where(x => x.Transaction is ManagerServer.Model.SalesInvoice && x.IsBalancing).ToDictionary(x => x.Transaction.Key, x => x.AccountAmount);
            var salesInvoiceBalances = salesInvoiceTransactions.GroupBy(x => x.SalesInvoice.Key).Select(x => new { x.Key, Balance = x.Sum(y => y.AccountAmount) }).ToDictionary(x => x.Key);

            var items = salesInvoices.Select(x => new Item()
            {
                Invoice = x.Key,
                Customer = x.Customer,
                BalanceDue = salesInvoiceBalances.ContainsKey(x.Key) ? salesInvoiceBalances[x.Key].Balance : 0m,
                InvoiceTotal = salesInvoiceAmounts.ContainsKey(x.Key) ? salesInvoiceAmounts[x.Key] : 0m,
            }).OrderBy(x => x.Customer.HasValue).ThenByDescending(x => x.BalanceDue < 0m).ThenByDescending(x => x.BalanceDue != 0m).ToArray();

            var salesInvoicesWithEarlyPaymentDiscountApplied = new HashSet<Guid>(database.OfType<ManagerServer.Model.CreditNote>().Where(x => x.Type == ManagerServer.Model.Enums.CreditNoteType.EarlyPaymentDiscount && x.Customer.HasValue && x.SalesInvoice.HasValue).Select(x => x.SalesInvoice.Value).Distinct().ToArray());
            foreach (var e in items)
            {
                if (e.InvoiceTotal <= e.BalanceDue) continue;

                var salesInvoice = salesInvoicesByKey[e.Invoice];
                if (!salesInvoice.EarlyPaymentDiscount) continue;
                if (e.InvoiceTotal <= 0m) continue;
                if (!salesInvoice.Customer.HasValue) continue;
                var dueDateForEarlyPaymentDiscount = salesInvoice.IssueDate.AddDays(salesInvoice.EarlyPaymentDiscountDays ?? 0);

                if (salesInvoicesWithEarlyPaymentDiscountApplied.Contains(e.Invoice)) continue;

                var currency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(database.SingleOrDefault<ManagerServer.Model.Customer>(e.Customer)?.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                var decimalDigits = currency.GetDecimalPlaces();

                var earlyPaymentDiscount = salesInvoice.EarlyPaymentDiscountAmount;
                if (salesInvoice.EarlyPaymentDiscountType == ManagerServer.Model.Enums.DiscountType.Percentage)
                {
                    earlyPaymentDiscount = Math.Round(e.InvoiceTotal / 100m * salesInvoice.EarlyPaymentDiscountRate, decimalDigits, MidpointRounding.AwayFromZero);
                }

                var balanceBeforeDueDate = salesInvoiceTransactions.Where(x => x.Date <= dueDateForEarlyPaymentDiscount && x.Customer.Key == salesInvoice.Customer.Value && x.SalesInvoice.Key == salesInvoice.Key).Sum(x => x.AccountAmount);
                if (balanceBeforeDueDate - earlyPaymentDiscount > 0m) continue;

                e.EarlyPaymentDiscount = earlyPaymentDiscount;
            }

            return items.Where(x => x.EarlyPaymentDiscount != 0m).ToArray();
        }
    }
}

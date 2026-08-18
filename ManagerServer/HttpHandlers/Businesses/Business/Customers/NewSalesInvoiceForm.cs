using System;
using System.Collections.Generic;
using ManagerServer.Model.Attributes;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Guid("3a91c0de-c12c-40c9-a92c-8b9e3a84e308")]
    [Title(nameof(Strings.Uninvoiced))]
    [Guide("The Uninvoiced screen displays all *billable time* and *billable expenses* that have been recorded but not yet included in a sales invoice.")]
    [Guide("This screen helps you track unbilled work and expenses for a specific customer.")]
    [Header("Creating a Sales Invoice")]
    [Guide("To create a sales invoice from uninvoiced items:")]
    [Guide("1. Select the items you want to include by checking the boxes next to them")]
    [Guide("2. Click the **New Sales Invoice** button at the bottom of the screen")]
    [Guide("3. The system will automatically generate a sales invoice with the selected items")]
    [Header("Understanding the Display")]
    [Guide("Each row shows important details about the uninvoiced item, including the date, description, amount, and current status.")]
    [Guide("Items marked as *Uninvoiced* in red indicate they are available for billing.")]
    [Columns]
    internal sealed class NewSalesInvoiceForm : NakedObjectsWithCustomFields<NewSalesInvoiceForm.Item>
    {
        [ProtoMember(1)] public Guid Customer;

        protected override void InnerGet4(Context context)
        {
            var list = new List<Item>();

            var customer = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Customer>(Customer);
            var currency = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.ForeignCurrency>(customer?.Currency) as ManagerServer.Model.Currency ?? ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
            var billableTime = BillableTime.BillableTime.GetBillableTime(Business).Where(x => x.CustomerKey == Customer && x.Status == BillableTime.BillableTime.Status.Uninvoiced).ToArray();
            var billableExpenses = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.BillableExpensesAssetAccount && x.Customer.Key == Customer && x.SalesInvoice == null).ToArray();

            list.AddRange(billableTime.Select(x => new Item() { Date = x.Date, ItemType = ItemType.BillableTime, Key = x.Key, Description = x.Description, Amount = x.Amount, TrackingCode = x.TrackingCodeKey }));
            list.AddRange(billableExpenses.Select(x => new Item() { Date = x.Date, ItemType = ItemType.BillableExpense, Key = x.Transaction.Key, Description = x.Description, Amount = x.AccountAmount, TrackingCode = x.Division?.Key, LineNumber = x.LineNumber }));

            context.Set<Array>(list.ToArray());
            context.Set(new BatchOperation() { Name = Strings.NewSalesInvoice });

            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(Item[] rows)
        {
            var list = new List<Tuple<string, byte[]>>();
            foreach (var e in rows)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, e);
                    list.Add(new Tuple<string, byte[]>(nameof(NewSalesInvoiceForm), ms.ToArray()));
                }
            }
            return list.ToArray();
        }

        public override BusinessTemplate[] GetEdit(Item[] rows)
        {
            var referrer = this.ToUrl();

            var output = new BusinessTemplate[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].ItemType == ItemType.BillableTime) output[i] = new BillableTime.BillableTimeEntryForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                if (rows[i].ItemType == ItemType.BillableExpense)
                {
                    var o2 = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Transaction>(rows[i].Key);
                    if (o2 is ManagerServer.Model.Payment) output[i] = new Payments.PaymentForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                    if (o2 is ManagerServer.Model.Receipt) output[i] = new Receipts.ReceiptForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                    if (o2 is ManagerServer.Model.JournalEntry) output[i] = new JournalEntries.JournalEntryForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                    if (o2 is ManagerServer.Model.ExpenseClaim) output[i] = new ExpenseClaims.ExpenseClaimForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                    if (o2 is ManagerServer.Model.DebitNote) output[i] = new DebitNotes.DebitNoteForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                    if (o2 is ManagerServer.Model.PurchaseInvoice) output[i] = new PurchaseInvoices.PurchaseInvoiceForm() { Business = Business, Key = rows[i].Key, Referrer = referrer };
                }
            }

            return output;
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("3ef0421a-0abd-46c9-bd5c-60963b8566f2")]
        [Guide("Displays the date when the *billable time* was worked or the *billable expense* was incurred.")]
        [Guide("This date will be included in the line description when the item is invoiced.")]
        public DateTime[] GetDate(Item[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("af762f5a-b22a-44fd-80ad-416399ad4163")]
        [Guide("Shows the description entered for the *billable time* entry or *billable expense*.")]
        [Guide("This description will appear on the sales invoice line item to help the customer understand what they are being charged for.")]
        public string[] GetDescription(Item[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("23eb1f93-937a-465a-831f-605505e791b8")]
        [Guide("Displays the amount that will be charged to the customer for each item.")]
        [Guide("For *billable time*, this is calculated as the hourly rate multiplied by the hours worked.")]
        [Guide("For *billable expenses*, this shows the expense amount that was marked as billable.")]
        [Guide("The total at the bottom shows the sum of all displayed items.")]
        public decimal[] GetAmount(Item[] rows)
        {
            return rows.Select(x => x.Amount).ToArray();
        }

        [Default]
        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("03fa42a7-4569-4d19-a6ef-c52eea58d5b4")]
        [Guide("Shows the current billing status of each item.")]
        [Guide("Items displayed as *Uninvoiced* (in red) are ready to be included in a sales invoice.")]
        [Guide("Once items are invoiced, they will no longer appear on this screen.")]
        public ItemStatus[] GetStatus(Item[] rows)
        {
            return rows.Select(x => x.Status).ToArray();
        }

        [ProtoContract]
        public sealed class Item
        {
            [ProtoMember(1)] public Guid Key;
            [ProtoMember(2)] public int? LineNumber;
            [ProtoMember(3)] public decimal Amount;
            [ProtoMember(4)] public string Description;
            [ProtoMember(5)] public DateTime Date;

            [ProtoMember(7)] public Guid? TrackingCode;
            [ProtoMember(6)] public ItemType ItemType;
            public ItemStatus Status;
        }

        public enum ItemType
        {
            BillableTime,
            BillableExpense
        }

        public enum ItemStatus
        {
            [Danger] Uninvoiced
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey(nameof(NewSalesInvoiceForm)))
                {
                    var pendingEarlyPaymentDiscounts = form[nameof(NewSalesInvoiceForm)].ToString();
                    if (!string.IsNullOrWhiteSpace(pendingEarlyPaymentDiscounts))
                    {
                        var items = pendingEarlyPaymentDiscounts.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        SetCulture(Business);

                        var customer = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Customer>(Customer);
                        var defaultTaxCode = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountBillableTimeInvoiced>().DefaultTaxCode;
                        var salesInvoiceLines = new List<ManagerServer.Model.SalesInvoice.Line>();
                        var salesInvoiceKey = Guid.CreateVersion7();

                        var list = new List<ManagerServer.Model.Object>();

                        foreach (var e in items)
                        {
                            using (var ms = new System.IO.MemoryStream(e))
                            {
                                var e2 = ProtoBuf.Serializer.Deserialize<Item>(ms);

                                if (e2.ItemType == ItemType.BillableExpense && e2.LineNumber.HasValue)
                                {
                                    var journalEntry = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.JournalEntry>(e2.Key);
                                    if (journalEntry != null)
                                    {
                                        journalEntry.Lines[e2.LineNumber.Value].BillableExpenseSalesInvoice = salesInvoiceKey;
                                        list.Add(journalEntry);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced, LineDescription = e2.Date.ToLocalShortDisplayString() + " - " + e2.Description, SalesUnitPrice = e2.Amount, TaxCode = defaultTaxCode, Division = e2.TrackingCode });
                                    }

                                    var expenseClaim = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.ExpenseClaim>(e2.Key);
                                    if (expenseClaim != null)
                                    {
                                        expenseClaim.Lines[e2.LineNumber.Value].BillableExpenseSalesInvoice = salesInvoiceKey;
                                        list.Add(expenseClaim);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced, LineDescription = e2.Date.ToLocalShortDisplayString() + " - " + e2.Description, SalesUnitPrice = e2.Amount, TaxCode = defaultTaxCode, Division = e2.TrackingCode });
                                    }

                                    var receipt = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Receipt>(e2.Key);
                                    if (receipt != null)
                                    {
                                        receipt.Lines[e2.LineNumber.Value].BillableExpenseSalesInvoice = salesInvoiceKey;
                                        list.Add(receipt);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced, LineDescription = e2.Date.ToLocalShortDisplayString() + " - " + e2.Description, SalesUnitPrice = e2.Amount, TaxCode = defaultTaxCode, Division = e2.TrackingCode });
                                    }

                                    var payment = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Payment>(e2.Key);
                                    if (payment != null)
                                    {
                                        payment.Lines[e2.LineNumber.Value].BillableExpenseSalesInvoice = salesInvoiceKey;
                                        list.Add(payment);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced, LineDescription = e2.Date.ToLocalShortDisplayString() + " - " + e2.Description, SalesUnitPrice = e2.Amount, TaxCode = defaultTaxCode, Division = e2.TrackingCode });
                                    }

                                    var purchaseInvoice = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.PurchaseInvoice>(e2.Key);
                                    if (purchaseInvoice != null)
                                    {
                                        purchaseInvoice.Lines[e2.LineNumber.Value].BillableExpenseSalesInvoice = salesInvoiceKey;
                                        list.Add(purchaseInvoice);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced, LineDescription = e2.Date.ToLocalShortDisplayString() + " - " + e2.Description, SalesUnitPrice = e2.Amount, TaxCode = defaultTaxCode, Division = e2.TrackingCode });
                                    }

                                    var debitNote = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.DebitNote>(e2.Key);
                                    if (debitNote != null)
                                    {
                                        debitNote.Lines[e2.LineNumber.Value].BillableExpenseSalesInvoice = salesInvoiceKey;
                                        list.Add(debitNote);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableExpensesInvoiced, LineDescription = e2.Date.ToLocalShortDisplayString() + " - " + e2.Description, SalesUnitPrice = e2.Amount, TaxCode = defaultTaxCode, Division = e2.TrackingCode });
                                    }
                                }

                                if (e2.ItemType == ItemType.BillableTime)
                                {
                                    var billableTime = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.BillableTime>(e2.Key);
                                    if (billableTime != null)
                                    {
                                        billableTime.Status = ManagerServer.Model.Enums.BillableTimeStatus.Invoiced;
                                        billableTime.SalesInvoice = salesInvoiceKey;
                                        list.Add(billableTime);
                                        salesInvoiceLines.Add(new ManagerServer.Model.SalesInvoice.Line() { Account = ManagerServer.Model.Master.AccountKeys.BillableTimeInvoiced, LineDescription = billableTime.Date.ToLocalShortDisplayString() + " - " + billableTime.Description, Qty = billableTime.GetQty(), SalesUnitPrice = billableTime.HourlyRate, TaxCode = defaultTaxCode, Division = e2.TrackingCode, CustomFields = billableTime.CustomFields });
                                    }
                                }
                            }
                        }

                        var salesInvoice = ProtoBuf.Serializer.DeepClone<ManagerServer.Model.SalesInvoice>(ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.SalesInvoice>());
                        salesInvoice.Key = salesInvoiceKey;
                        salesInvoice.Customer = Customer;
                        salesInvoice.HasLineDescription = true;
                        salesInvoice.BillingAddress = customer.BillingAddress;
                        salesInvoice.IssueDate = DateTime.Today;
                        salesInvoice.Lines = salesInvoiceLines.ToArray();
                        if (salesInvoice.AutomaticReference)
                        {
                            salesInvoice.Reference = GetNextSalesInvoiceReferenceNumber().ToString();
                            salesInvoice.AutomaticReference = false;
                        }

                        list.Add(salesInvoice);

                        ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());

                        Response.Redirect(new SalesInvoices.SalesInvoiceView() { Business = Business, Key = salesInvoice.Key }.ToUrl());
                    }
                }
            }
            await base.InnerPost();
        }

        private long GetNextSalesInvoiceReferenceNumber()
        {
            var references = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.SalesInvoice>().Where(x => !string.IsNullOrWhiteSpace(x.Reference)).Select(x => x.Reference).ToArray();
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
    }
}
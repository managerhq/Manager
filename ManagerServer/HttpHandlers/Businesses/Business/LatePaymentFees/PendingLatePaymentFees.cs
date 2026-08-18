using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    [Guid("feedb866-805b-4f2d-97a9-2085785b01e5")]
    [Title(nameof(Strings.LatePaymentFees))]
    [Guide("The **Late Payment Fees** screen displays overdue sales invoices that are eligible for *late payment charges*.")]
    [Guide("This screen automatically identifies invoices that meet the criteria for *late payment fees* based on your configured settings.")]
    [Header("How it works")]
    [Guide("*Late payment fees* are calculated as a percentage of the outstanding invoice balance.")]
    [Guide("The system checks each overdue invoice and calculates fees based on the *percentage rate* specified in the sales invoice settings.")]
    [Guide("Fees are generated monthly after the initial *due date* has passed.")]
    [Header("Creating late payment fees")]
    [Guide("Select one or more invoices from the list by checking the boxes next to them.")]
    [Guide("Click the **New Late Payment Fee** button to generate the fees for the selected invoices.")]
    [Guide("The fees will be automatically created and linked to the corresponding sales invoices.")]
    [Columns]
    internal sealed class PendingLatePaymentFees : NakedObjectsWithCustomFields<PendingLatePaymentFees.Item>
    {
        [ProtoContract]
        public sealed class Item
        {
            [ProtoMember(1)] public DateTime Date;
            [ProtoMember(3)] public Guid? Customer;
            [ProtoMember(4)] public Guid? SalesInvoice;
            [ProtoMember(5)] public decimal BalanceDue;
            [ProtoMember(6)] public decimal LatePaymentFee;
            [ProtoMember(7)] public decimal ExchangeRate;
            [ProtoMember(8)] public bool ExchangeRateIsInverse;
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(Item[] rows)
        {
            var list = new List<Tuple<string, byte[]>>();
            foreach (var e in rows)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, e);
                    list.Add(new Tuple<string, byte[]>("PendingLatePaymentFees", ms.ToArray()));
                }
            }
            return list.ToArray();
        }

        public override BusinessTemplate[] GetEdit(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesInvoices.SalesInvoiceForm() { Business = Business, Key = x.SalesInvoice.Value, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesInvoices.SalesInvoiceView() { Business = Business, Key = x.SalesInvoice.Value, Referrer = referrer }).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("d2c2e114-2bf3-4d87-a15c-11d23ddc783f")]
        [Guide("The date when the *late payment fee* becomes applicable for this invoice.")]
        [Guide("This is typically one day after the invoice *due date* for the first fee, then monthly thereafter.")]
        public DateTime[] GetDate(Item[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("8ba00381-fffb-470d-b3e2-c8d108fdb34e")]
        [Guide("The customer who will be charged the *late payment fee*.")]
        [Guide("This is the customer associated with the overdue *sales invoice*.")]
        public string[] GetCustomer(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("e2b1cb30-9cef-4350-9cc7-8d00191fbd1e")]
        [Guide("The overdue *sales invoice* that is triggering the *late payment fee*.")]
        [Guide("Click on the invoice number to view or edit the original invoice details.")]
        public string[] GetSalesInvoice(Item[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.SalesInvoice>(x.SalesInvoice)?.GetName()).ToArray();
        }

        [Default]
        [Guid("3f88f285-1022-41d4-9f67-cc9e47b5d6ef")]
        [Guide("The outstanding balance on which the *late payment fee* is calculated.")]
        [Guide("This amount represents the unpaid portion of the invoice at the time the fee is being generated.")]
        public decimal[] GetBalanceDue(Item[] rows)
        {
            return rows.Select(x => x.BalanceDue).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("4067513e-6d65-44ab-b89e-3b3d1eb30648")]
        [Guide("The calculated *late payment fee* amount that will be charged to the customer.")]
        [Guide("This fee is automatically calculated based on the *percentage rate* configured in the sales invoice settings.")]
        public decimal[] GetNewLatePaymentFee(Item[] rows)
        {
            return rows.Select(x => x.LatePaymentFee).ToArray();
        }

        public override int GetContextCount()
        {
            return GetItems().Length;
        }

        protected override void InnerGet4(Context context)
        {
            context.Set<Array>(GetItems());
            context.Set(new BatchOperation() { Name = Strings.NewLatePaymentFee });

            base.InnerGet4(context);
        }

        private Item[] GetItems()
        {
            var items = new List<Item>();

            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var salesInvoiceBalances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices().Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.SalesInvoice != null).GroupBy(x => x.SalesInvoice.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount));
            var latePaymentFees = database.OfType<ManagerServer.Model.LatePaymentFee>().Where(x => x.SalesInvoice.HasValue).GroupBy(x => x.SalesInvoice.Value).ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.Date).First().Date);
            var salesInvoicesWithLatePaymentFees = database.OfType<SalesInvoice>().Where(x => !x.ClosedInvoice && x.LatePaymentFees && x.LatePaymentFeesPercentage > 0m).ToArray();
            foreach (var e in salesInvoicesWithLatePaymentFees)
            {
                if (e.Key == new Guid("ad12b60b-23bf-4421-94df-8be79cef533e")) continue; // this line can bee removed when OfType stops returning form default objects

                var nextLatePaymentFeeDate = e.GetDueDate().AddDays(1);
                if (latePaymentFees.ContainsKey(e.Key)) nextLatePaymentFeeDate = latePaymentFees[e.Key].Date.AddMonths(1);
                if (nextLatePaymentFeeDate > DateTime.Today) continue;

                if (salesInvoiceBalances.TryGetValue(e.Key, out decimal balanceDue))
                {
                    if (balanceDue <= 0m) continue;

                    var foreignCurrency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(database.SingleOrDefault<ManagerServer.Model.Customer>(e.Customer)?.Currency);
                    var currency = foreignCurrency as ManagerServer.Model.Currency ?? baseCurrency;
                    var decimalDigits = currency.GetDecimalPlaces();

                    var latePaymentFeeAmount = Math.Round(balanceDue / 100m * e.LatePaymentFeesPercentage, decimalDigits, MidpointRounding.AwayFromZero);

                    items.Add(new Item()
                    {
                        Date = nextLatePaymentFeeDate,
                        Customer = e.Customer,
                        SalesInvoice = e.Key,
                        BalanceDue = balanceDue,
                        LatePaymentFee = latePaymentFeeAmount
                    });
                }
            }

            return items.ToArray();
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey("PendingLatePaymentFees"))
                {
                    var item = form["PendingLatePaymentFees"].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var items = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        SetCulture(Business);

                        var list = new List<ManagerServer.Model.Object>();

                        foreach (var e in items)
                        {
                            using (var ms = new System.IO.MemoryStream(e))
                            {
                                var e2 = ProtoBuf.Serializer.Deserialize<Item>(ms);

                                list.Add(new ManagerServer.Model.LatePaymentFee()
                                {
                                    Date = e2.Date,
                                    Amount = e2.LatePaymentFee,
                                    Customer = e2.Customer,
                                    SalesInvoice = e2.SalesInvoice
                                });
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
    }
}

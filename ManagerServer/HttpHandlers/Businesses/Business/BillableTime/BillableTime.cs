using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.BillableTime
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("d0c4e06e-dd0b-419e-8883-89f3e7e7bf7b")]
    [Title(nameof(Strings.BillableTime))]
    [Guide("The `Billable Time` tab helps businesses track time spent on client work for invoicing purposes.")]
    [Guide("Record hours worked, assign them to customers, and convert them into invoices when ready.")]
    [TabScreenshot("fa-stopwatch", nameof(Strings.BillableTime))]
    [Header("Recording Billable Time")]
    [Guide("Click the `New Billable Time` button to record time spent on client work.")]
    [HeroButtonScreenshot(nameof(Strings.BillableTime), nameof(Strings.NewBillableTime))]
    [LinkGuide("Learn about recording time:", typeof(BillableTimeEntryForm))]
    [Header("Converting Time to Invoices")]
    [Guide("New billable time entries are marked as `Uninvoiced` by default.")]
    [Guide("To invoice recorded time, go to the `Customers` tab and click the amount in the `Uninvoiced` column.")]
    [Guide("From there, create a `New Sales Invoice` that includes the uninvoiced time.")]
    [LinkGuide("Learn about customer invoicing:", typeof(Customers.Customers))]
    [Header("Writing Off Unbillable Time")]
    [Guide("If time won't be invoiced, mark it as written off:")]
    [Guide("1. Click `Edit` on the time entry")]
    [Guide("2. Change `Status` to `Written-off`")]
    [Guide("3. Enter the write-off date")]
    [Guide("This removes the time from your billable assets while maintaining accurate records.")]
    [Header("Managing and Analyzing Time Entries")]
    [Guide("The `Billable Time` tab displays the following information:")]
    [Columns]
    [Guide("Click `Edit Columns` to customize visible columns.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn about column customization:", typeof(NakedObjectsWithEditColumns<BillableTime>))]
    [Header("Advanced Reporting")]
    [Guide("Use `Advanced Queries` to filter and group billable time for better insights.")]
    [Guide("Example: View uninvoiced hours grouped by customer:")]
    [AdvancedQuery(select: new[] { nameof(Strings.Customer), nameof(Strings.Amount) }, where: new[] { nameof(Strings.Status), nameof(Strings.Is), nameof(Strings.Uninvoiced) }, groupBy: new[] { nameof(Strings.Customer) })]
    [LinkGuide("Learn about advanced queries:", typeof(NakedObjectsWithAdvancedQueries))]
    [Header("Custom Fields for Enhanced Tracking")]
    [Guide("Add custom fields to track additional information like staff member names or project codes.")]
    [Guide("This enables filtering and grouping by these custom attributes in reports.")]
    [LinkGuide("Learn about custom fields:", typeof(Settings.CustomFields.CustomFields))]
    internal sealed class BillableTime : NakedObjectsWithAutomaticRows<ManagerServer.Model.BillableTime>
    {
        [Default]
        [Center]
        [WarnIfFutureDate]
        [MinWidth]
        [WhitespaceNoWrap]
        [Guid("d8a5c2f1-8a7a-45e7-9862-3e90bcc8c12e")]
        [Guide("The date when the billable work was performed. This date is used for tracking when services were provided and for reporting purposes.")]
        public DateTime[] GetDate(ManagerServer.Model.BillableTime[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("89720bfd-37a3-4e7a-9e5c-5ba9d57f890a")]
        [Guide("The name of the customer to whom this billable time is charged.")]
        public string[] GetCustomer(ManagerServer.Model.BillableTime[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.Name).ToArray();
        }

        [Default]
        [Guid("aeeb703e-9ff0-49ca-bca1-1f9a79a3be42")]
        [Guide("A detailed description of the work performed. This helps identify what services were provided and can be included on invoices.")]
        public string[] GetDescription(ManagerServer.Model.BillableTime[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default]
        [Center]
        [Guid("295ef6ec-da39-4128-be7b-adf47b331aa1")]
        [Guide("The duration of time spent on the billable work, displayed in hours and minutes format.")]
        public string[] GetTimeSpent(ManagerServer.Model.BillableTime[] rows)
        {
            return rows.Select(x => GetTimeSpent(x.TimeSpent, x.TimeSpentMinutes)).ToArray();
        }

        [Guid("9ec88eb5-5329-403f-a197-8eaed23647f7")]
        [Guide("The division or department to which this billable time is allocated for tracking and reporting purposes.")]
        public string[] GetDivision(ManagerServer.Model.BillableTime[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Division>(x.Division)?.Name).ToArray();
        }

        [Bold]
        [Right, Sum]
        [Default]
        [Guid("24063f68-aa01-4134-922e-76b267a66f2b")]
        [Guide("The total billable amount calculated by multiplying the time spent by the hourly rate. This represents the value of services to be invoiced.")]
        public Tuple<decimal, Currency>[] GetAmount(ManagerServer.Model.BillableTime[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var customer = database.SingleOrDefault<Customer>(e.Customer);
                var currency = database.SingleOrDefault<ForeignCurrency>(customer?.Currency) as Currency ?? baseCurrency;
                var amount = e.GetAmount(currency.GetDecimalPlaces());
                output.Add(new Tuple<decimal, Currency>(amount, currency));
            }
            return output.ToArray();
        }

        [Default]
        [Center]
        [MinWidth]
        [WhitespaceNoWrap]
        [Guid("a07660a8-60f9-4535-adc4-d52fb3296d36")]
        [Guide("The `Status` column shows the current state of each billable time entry:")]
        [Guide("• `Uninvoiced` - Time recorded but not yet billed to the customer")]
        [Guide("• `Invoiced` - Time included on a sales invoice and billed")]
        [Guide("• `Written-off` - Time that won't be billed and has been removed from assets")]
        public Status[] GetStatus(ManagerServer.Model.BillableTime[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Status>();
            foreach (var e in rows)
            {
                if (e.Status == ManagerServer.Model.Enums.BillableTimeStatus.Invoiced && database.SingleOrDefault<ManagerServer.Model.SalesInvoice>(e.SalesInvoice) != null)
                {
                    output.Add(Status.Invoiced);
                }
                else if (e.Status == ManagerServer.Model.Enums.BillableTimeStatus.WrittenOff && e.WrittenOffDate.HasValue)
                {
                    output.Add(Status.WrittenOff);
                }
                else
                {
                    output.Add(Status.Uninvoiced);
                }
            }
            return output.ToArray();
        }       

        public static string GetTimeSpent(int? hours, int? minutes)
        {
            string timeSpent = null;
            if (hours.HasValue && hours.Value != 0)
            {
                timeSpent = string.Format(Strings.XxxHours, hours.Value.ToString());
            }
            if (minutes.HasValue && minutes.Value != 0)
            {
                if (timeSpent != null) timeSpent += "&nbsp;";
                timeSpent += string.Format(Strings.XxxMinutes, minutes.Value.ToString());
            }
            return timeSpent;
        }

        public static Item[] GetBillableTime(string entityId)
        {
            var trackingCodes = ApplicationData.Instance.Businesses.Get(entityId).OfType<ManagerServer.Model.Division>().ToDictionary(x => x.Key);
            var salesInvoices = ApplicationData.Instance.Businesses.Get(entityId).OfType<SalesInvoice>().ToDictionary(x => x.Key);
            var customers = ApplicationData.Instance.Businesses.Get(entityId).OfType<ManagerServer.Model.Customer>().ToDictionary(x => x.Key);
            var foreginCurrencies = ApplicationData.Instance.Businesses.Get(entityId).OfType<ForeignCurrency>().ToDictionary(x => x.Key);
            var baseCurrencyDigits = ApplicationData.Instance.Businesses.Get(entityId).Single<ManagerServer.Model.BaseCurrency>().GetDecimalPlaces();

            var list = new List<Item>();

            foreach (var e in ApplicationData.Instance.Businesses.Get(entityId).OfType<ManagerServer.Model.BillableTime>().ToArray())
            {
                Customer customer = null;
                if (e.Customer.HasValue && customers.ContainsKey(e.Customer.Value)) customer = customers[e.Customer.Value];
                var currency = customer?.Currency;
                var numberDecimalDigits = baseCurrencyDigits;
                if (currency.HasValue && foreginCurrencies.ContainsKey(currency.Value)) numberDecimalDigits = foreginCurrencies[currency.Value].GetDecimalPlaces();

                var status = Status.Uninvoiced;
                if (e.Status == ManagerServer.Model.Enums.BillableTimeStatus.Invoiced && e.SalesInvoice.HasValue && e.Customer.HasValue && salesInvoices.ContainsKey(e.SalesInvoice.Value)) status = Status.Invoiced;
                else if (e.Status == ManagerServer.Model.Enums.BillableTimeStatus.WrittenOff && e.WrittenOffDate.HasValue && e.WrittenOffDate.Value >= e.Date) status = Status.WrittenOff;

                list.Add(new Item()
                {
                    Key = e.Key,
                    Date = e.Date,
                    Description = e.Description,
                    Currency = currency,
                    CustomerKey = (customer != null ? customer.Key : default(Guid?)),
                    CustomerName = (customer != null ? customer.Name : null),
                    HourlyRate = e.HourlyRate,
                    Hours = e.TimeSpent,
                    Minutes = e.TimeSpentMinutes,
                    Amount = e.GetAmount(numberDecimalDigits),
                    Status = status,
                    TrackingCodeKey = e.Division,
                    TrackingCodeName = (e.Division.HasValue && trackingCodes.ContainsKey(e.Division.Value) ? trackingCodes[e.Division.Value].Name : null),
                    CustomFields = e.CustomFields,
                    CustomFields2 = e.CustomFields2
                });
            }

            return list.ToArray();
        }

        public sealed class Item
        {
            public Guid Key;
            public DateTime Date;
            public Guid? CustomerKey;
            public string CustomerName;
            public Guid? Currency;
            public int? Hours;
            public int? Minutes;
            public decimal? HourlyRate;
            public decimal Amount;
            public string Description;
            public Status Status;
            public Guid? TrackingCodeKey;
            public string TrackingCodeName;
            public Dictionary<Guid, string> CustomFields;
            public CustomFields CustomFields2;
        }

        public enum Status
        {
            [Danger] Uninvoiced,
            [Success] Invoiced,
            WrittenOff
        }
    }
}

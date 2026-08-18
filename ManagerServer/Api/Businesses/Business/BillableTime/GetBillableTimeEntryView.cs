using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.BillableTime
{
    [ProtoContract]
    internal sealed class GetBillableTimeEntryView : GetTransactionView<Model.BillableTime>
    {
        private static string GetTimeSpent(int? hours, int? minutes)
        {
            string timeSpent = null;
            if (hours.HasValue && hours.Value != 0) timeSpent = string.Format(Strings.XxxHours, hours.Value.ToString());
            if (minutes.HasValue && minutes.Value != 0)
            {
                if (timeSpent != null) timeSpent += " ";
                timeSpent += string.Format(Strings.XxxMinutes, minutes.Value.ToString());
            }
            return timeSpent;
        }

        protected override TransactionView GetViewData(Model.BillableTime o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.Billable_time;

            if (!o.Customer.HasValue) return viewData;

            var customer = Database.SingleOrDefault<Model.Customer>(o.Customer.Value);
            if (customer == null) return viewData;

            viewData.recipient.code = customer.Code;
            viewData.recipient.name = customer.Name;
            viewData.recipient.address = customer.BillingAddress;
            viewData.recipient.email = customer.Email;

            var currency = customer.Currency;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.HourlyRate, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.TimeSpent, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Amount, align = "right", nowrap = true });

            var currencies = Query.Currencies.GetCurrencyProvider(Business);
            var decimalDigits = currencies.Get(currency).GetDecimalPlaces();

            var row = new TransactionView.Row();
            row.cells.Add(new TransactionView.Cell { text = o.Description });
            row.cells.Add(new TransactionView.Cell { text = o.HourlyRate.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            row.cells.Add(new TransactionView.Cell { text = GetTimeSpent(o.TimeSpent, o.TimeSpentMinutes) });
            row.cells.Add(new TransactionView.Cell { text = o.GetAmount(decimalDigits).ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            viewData.table.rows.Add(row);

            var invoiced = o.Status == Model.Enums.BillableTimeStatus.Invoiced && o.SalesInvoice.HasValue && Database.SingleOrDefault<Model.SalesInvoice>(o.SalesInvoice.Value) != null;
            var writtenOff = o.Status == Model.Enums.BillableTimeStatus.WrittenOff && o.WrittenOffDate.HasValue && o.WrittenOffDate.Value >= o.Date;

            if (writtenOff) viewData.emphasis = new TransactionView.Emphasis { text = Strings.WrittenOff };
            else if (invoiced) viewData.emphasis = new TransactionView.Emphasis { text = Strings.Invoiced, positive = true };
            else viewData.emphasis = new TransactionView.Emphasis { text = Strings.Uninvoiced, negative = true };

            if (o.Division.HasValue)
            {
                var division = Database.SingleOrDefault<Model.Division>(o.Division.Value);
                if (division != null)
                {
                    viewData.custom_fields.Add(new TransactionView.CustomField { label = Strings.Division, text = division.Name });
                }
            }

            viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
            viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));

            return viewData;
        }
    }
}

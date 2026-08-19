using Markdig;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringSalesQuote))]
    [Guide("Create sales quotes that repeat on a regular schedule.")]
    [Guide("Useful for regular pricing updates or periodic service quotations.")]
    [Fields(typeof(ManagerServer.Model.RecurringSalesQuote))]
    internal sealed class RecurringSalesQuoteForm : NakedVueForm<ManagerServer.Model.RecurringSalesQuote>
    {
        protected override void OnSource(ManagerServer.Model.RecurringSalesQuote form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.SalesQuote salesQuote)
            {
                Copy(salesQuote, form);

                // Copy() only matches members by name; SalesQuote exposes CustomTheme/CustomThemeId
                // while this form exposes its own uniquely-named fields, so bridge via IHasCustomTheme.
                if (salesQuote is ManagerServer.Model.IHasCustomTheme sourceCustomTheme)
                {
                    form.HasSalesQuoteCustomTheme = sourceCustomTheme.CustomTheme;
                    form.SalesQuoteCustomTheme = sourceCustomTheme.CustomThemeId;
                }
            }
        }
    }
}

using ManagerServer.Helpers;
using System.Linq;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsOfPendingRecurringTransactions<T> : NakedObjectsWithAutomaticRows<T> where T : ManagerServer.Model.Object, ManagerServer.Model.IRecurringTransaction, new()
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).OfType<T>().Any();
        }

        public override int GetContextCount()
        {
            return ApplicationData.Businesses.Get(Business).OfType<T>().Count(x => x.CanBeIssued());
        }

        protected override T[] OnGetRows(T[] rows)
        {
            return rows.Where(x => x.CanBeIssued()).ToArray();
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(T[] rows)
        {
            return rows.Select(x => new Tuple<string, byte[]>("NakedObjectsOfPendingRecurringTransactions", x.Key.ToByteArray())).ToArray();
        }

        protected override void InnerGet4(Context context)
        {
            context.Set(new BatchOperation() { Name = Strings.BatchCreate });

            base.InnerGet4(context);
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey("NakedObjectsOfPendingRecurringTransactions"))
                {
                    var item = form["NakedObjectsOfPendingRecurringTransactions"].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var keys = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        var outputType = typeof(T).GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ManagerServer.Model.IRecurringTransactionFor<>)).GenericTypeArguments[0];

                        var list = new List<ManagerServer.Model.Object>();

                        foreach (var e in keys)
                        {
                            var key = new Guid(e);

                            var recurringTransaction = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(key) as ManagerServer.Model.IRecurringTransaction;
                            if (recurringTransaction == null) continue;
                            if (!recurringTransaction.NextIssueDate.HasValue) continue;
                            if (recurringTransaction.NextIssueDate.Value > DateTime.Today) continue;

                            var transaction = (ManagerServer.Model.IRecurringTransactionDestination)Activator.CreateInstance(outputType);
                            Copy(recurringTransaction, transaction);
                            transaction.Date = recurringTransaction.NextIssueDate.Value;

                            // Copy() only matches members by name, but each recurring transaction type exposes its own
                            // uniquely-named custom theme fields (e.g. HasPurchaseInvoiceCustomTheme), so this must be
                            // bridged explicitly via the shared IHasCustomTheme interface.
                            if (recurringTransaction is ManagerServer.Model.IHasCustomTheme recurringCustomTheme && transaction is ManagerServer.Model.IHasCustomTheme transactionCustomTheme)
                            {
                                transactionCustomTheme.CustomTheme = recurringCustomTheme.CustomTheme;
                                transactionCustomTheme.CustomThemeId = recurringCustomTheme.CustomThemeId;
                            }

                            ((ManagerServer.Model.Object)transaction).Key = Guid.CreateVersion7();
                            list.Add((ManagerServer.Model.Object)transaction);

                            if (recurringTransaction.PeriodType == ManagerServer.Model.Enums.Period.Day)
                            {
                                recurringTransaction.NextIssueDate = recurringTransaction.NextIssueDate.Value.AddDays(recurringTransaction.Interval ?? 1);
                            }
                            if (recurringTransaction.PeriodType == ManagerServer.Model.Enums.Period.Week)
                            {
                                recurringTransaction.NextIssueDate = recurringTransaction.NextIssueDate.Value.AddDays((recurringTransaction.Interval ?? 1) * 7);
                            }
                            if (recurringTransaction.PeriodType == ManagerServer.Model.Enums.Period.Month)
                            {
                                recurringTransaction.NextIssueDate = recurringTransaction.NextIssueDate.Value.AddMonths(recurringTransaction.Interval ?? 1);
                                if (recurringTransaction.MonthDay == ManagerServer.Model.Enums.MonthDay.OnTheLastDay)
                                {
                                    var tempDate = recurringTransaction.NextIssueDate.Value.AddMonths(1);
                                    var lastDayOfTheMonth = new DateTime(tempDate.Year, tempDate.Month, 1).AddDays(-1);
                                    recurringTransaction.NextIssueDate = lastDayOfTheMonth;
                                }
                            }

                            if (recurringTransaction.ExpirationType == ManagerServer.Model.Enums.ExpirationType.Custom && recurringTransaction.UntilDate.HasValue && recurringTransaction.NextIssueDate.HasValue && recurringTransaction.NextIssueDate.Value > recurringTransaction.UntilDate.Value)
                            {
                                recurringTransaction.NextIssueDate = null;
                            }

                            list.Add(recurringTransaction as ManagerServer.Model.Object);
                        }

                        long? nextReference = null;
                        foreach (var e in list.OfType<ManagerServer.Model.IHasAutomaticReference>())
                        {
                            if (e is T) continue;

                            if (e.AutomaticReference)
                            {
                                if (!nextReference.HasValue)
                                {
                                    nextReference = e.GetNextReference(ApplicationData.Businesses.Get(Business).UnorderedOfType<ManagerServer.Model.Object>().Where(x => x.GetType() == outputType).Cast<ManagerServer.Model.IHasAutomaticReference>());
                                }

                                e.AutomaticReference = false;
                                e.Reference = nextReference.ToString();
                                nextReference++;
                            }
                        }

                        if (list.OfType<ManagerServer.Model.IForeignCurrencyTransaction>().Any())
                        {
                            var database = ApplicationData.Businesses.Get(Business);
                            var foreignCurrencies = database.OfType<ManagerServer.Model.ForeignCurrency>().ToDictionary(x => x.Key);
                            var currencies = new Dictionary<Guid, ForeignCurrency>();
                            foreach (var e in database.OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => x.Currency.HasValue && foreignCurrencies.ContainsKey(x.Currency.Value))) currencies.Add(e.Key, foreignCurrencies[e.Currency.Value]);
                            foreach (var e in database.OfType<ManagerServer.Model.Customer>().Where(x => x.Currency.HasValue && foreignCurrencies.ContainsKey(x.Currency.Value))) currencies.Add(e.Key, foreignCurrencies[e.Currency.Value]);
                            foreach (var e in database.OfType<ManagerServer.Model.Supplier>().Where(x => x.Currency.HasValue && foreignCurrencies.ContainsKey(x.Currency.Value))) currencies.Add(e.Key, foreignCurrencies[e.Currency.Value]);
                            foreach (var e in database.OfType<ManagerServer.Model.Employee>().Where(x => x.Currency.HasValue && foreignCurrencies.ContainsKey(x.Currency.Value))) currencies.Add(e.Key, foreignCurrencies[e.Currency.Value]);
                            foreach (var e in foreignCurrencies) currencies.Add(e.Key, e.Value);

                            foreach (var e in list.OfType<ManagerServer.Model.IForeignCurrencyTransaction>())
                            {
                                if (!e.Currency.HasValue) continue;
                                if (currencies.TryGetValue(e.Currency.Value, out ForeignCurrency foreignCurrency))
                                {
                                    var latestExchangeRate = database.OfType<ExchangeRate>().Where(x => x.Currency == foreignCurrency.Key && x.Date <= e.Date).OrderByDescending(x => x.Date).FirstOrDefault();
                                    if (latestExchangeRate != null)
                                    {
                                        e.ExchangeRate = latestExchangeRate.ExchangeRateValue;
                                        e.ExchangeRateIsInverse = latestExchangeRate.ExchangeRateIsInverse;
                                    }
                                    else
                                    {
                                        e.ExchangeRate = 1m;
                                    }
                                }
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
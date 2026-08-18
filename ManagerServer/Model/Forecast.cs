using System;
using System.Collections.Generic;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("821030a6-9820-4cba-8879-eda07853b9a6")]
    public sealed class Forecast : ManagerServer.Model.Object
    {
        [Guide("Enter the starting date for this forecast. This is when the first forecasted transaction will occur.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Select how often this forecast should repeat. Choose 'Never' for one-time forecasts.")]
        [ProtoMember(4), NoWrap] public Repeat Repeat { get; set; }
        [Guide("Enter the growth percentage for each period. For example, enter 5 for 5% growth per period.")]
        [ProtoMember(7), IfEnumNot(nameof(Repeat), (int)Repeat.Never), Append("%"), Prepend("+")] public decimal Growth { get; set; }
        [Guide("Enter a description for this forecast. This will appear on forecast reports.")]
        [ProtoMember(2), Long] public string Description { get; set; }
        [Guide("Add line items for accounts and amounts to include in this forecast.")]
        [ProtoMember(3)] public Line[] Lines { get; set; }
        [Guide("Check this box to temporarily disable this forecast without deleting it.")]
        [ProtoMember(8)] public bool Inactive { get; set; }

        [ProtoContract]
        public sealed class Line
        {
            [Guide("Select the account for this forecast line. This determines where the forecasted amount will be posted.")]
            [ProtoMember(1), Autocomplete(typeof(IReceiptOrPaymentAccount), Subtext = nameof(BalanceSheetAccount.Group)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [Guide("Enter the amount for this forecast line. Positive amounts represent inflows, negative amounts represent outflows.")]
            [ProtoMember(2), NoPlaceholder, Sum] public decimal Amount { get; set; }
            //[ProtoMember(3), Autocomplete(typeof(TaxCode)), IfTrue(nameof(Account), nameof(NamedObject.TaxCodeEnabled)), Short] public Guid? TaxCode;
            //[ProtoMember(4), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division;
        }

        private DateTime? GetNextDate(DateTime? date)
        {
            if (!date.HasValue) return Date;
            switch (Repeat)
            {
                case Repeat.Never: return null;
                case Repeat.EveryDay: return date.Value.AddDays(1);
                case Repeat.EveryWeek: return date.Value.AddDays(7);
                case Repeat.EveryTwoWeeks: return date.Value.AddDays(14);
                case Repeat.EveryMonth: return date.Value.AddMonths(1);
                case Repeat.EveryTwoMonths: return date.Value.AddMonths(2);
                case Repeat.EveryThreeMonths: return date.Value.AddMonths(3);
                case Repeat.EverySixMonths: return date.Value.AddMonths(6);
                case Repeat.EveryYear: return date.Value.AddYears(1);
                default: throw new NotImplementedException();
            }
        }

        private decimal GetAmount(BaseCurrency baseCurrency, int index, decimal amount)
        {
            if (Growth == 0) return baseCurrency.Round(amount);
            var output = baseCurrency.Round(amount);
            for (int i = 0; i < index; i++)
            {
                try
                {
                    output += baseCurrency.Round(output * (Growth / 100));
                }
                catch (OverflowException)
                {
                    return 0m;
                }
            }

            return output;
        }

        public IEnumerable<ForecastTransaction> GetForecastTransactions(BaseCurrency baseCurrency, DateTime from, DateTime to)
        {
            var list = new List<ForecastTransaction>();
            if (!Inactive)
            {
                var date = default(DateTime?);
                var index = -1;
                while (true)
                {
                    date = GetNextDate(date);
                    index++;

                    if (!date.HasValue) break;
                    else if (date < from) continue;
                    else if (date > to) break;

                    foreach (var e in Lines)
                    {
                        if (!e.Account.HasValue) continue;

                        list.Add(new ForecastTransaction()
                        {
                            Key = Key,
                            Date = date.Value,
                            Description = Description,
                            Account = e.Account.Value,
                            Amount = GetAmount(baseCurrency, index, e.Amount) * -1m
                        });
                    }
                }
            }
            return list;
        }

        public sealed class ForecastTransaction
        {
            public Guid Key { get; set; }
            public DateTime Date { get; set; }
            public Guid Account { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
        }
    }
}

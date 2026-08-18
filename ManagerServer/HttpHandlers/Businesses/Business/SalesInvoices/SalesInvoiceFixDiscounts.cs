using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoices), nameof(Strings.Discount))]
    [Guide("The **Fix Discounts** utility corrects rounding issues that can occur when using percentage-based discounts on sales invoices.")]
    [Guide("When applying percentage discounts, small rounding differences can accumulate and cause discrepancies in invoice totals. This utility identifies these discrepancies and converts the percentage discounts to exact amount discounts that preserve the intended final values.")]
    [Guide("This ensures that your invoice calculations remain accurate and consistent, especially when dealing with multiple line items or currencies with different decimal place requirements.")]
    [Guide("The utility will automatically scan all sales invoices with percentage discounts and convert them to exact amounts where rounding issues are detected.")]
    internal sealed class SalesInvoiceFixDiscounts : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            var salesInvoices = new List<Tuple<SalesInvoice, SalesInvoice.Line, bool, decimal>>();

            var before = new DateTime(2024, 03, 29).Ticks;
            foreach (var e in database.OfType<ManagerServer.Model.SalesInvoice>())
            {
                if (e.Lines == null) continue;
                if (!e.Discount) continue;
                if (e.DiscountType != ManagerServer.Model.Enums.DiscountType.Percentage) continue;
                //if (e.Timestamp > before) continue;

                var currency = database.SingleOrDefault<ForeignCurrency>(database.SingleOrDefault<Customer>(e.Customer)?.Currency) as Currency ?? baseCurrency;

                foreach (var e2 in e.Lines)
                {
                    if (e2.DiscountPercentage <= 0m) continue;

                    var lineTotalBeforeDiscount = e2.GetLineTotal(e);
                    var lineTotalBeforeDiscountRounded = currency.Round(lineTotalBeforeDiscount);

                    var oldLineTotalAfterDiscount = currency.Round(lineTotalBeforeDiscount / 100m * (100m - e2.GetDiscountPercentage(e).Value));
                    var newLineTotalAfterDiscount = currency.Round(lineTotalBeforeDiscountRounded / 100m *(100m - e2.GetDiscountPercentage(e).Value));

                    var fixedDiscount = lineTotalBeforeDiscountRounded - oldLineTotalAfterDiscount;
                    
                    if (oldLineTotalAfterDiscount != newLineTotalAfterDiscount)
                    {
                        Write($"#{e.Reference} ---- {lineTotalBeforeDiscount} && {lineTotalBeforeDiscountRounded} || {oldLineTotalAfterDiscount} != {newLineTotalAfterDiscount} || {e2.GetDiscountPercentage(e).Value}% => => {fixedDiscount}");
                        Br();
                    }

                    salesInvoices.Add(new Tuple<SalesInvoice, SalesInvoice.Line, bool, decimal>(e, e2, oldLineTotalAfterDiscount != newLineTotalAfterDiscount, fixedDiscount));
                }
            }

            Write("Calculated lines = "+salesInvoices.Count.ToString());
            Br();

            var objectsToUpdate = new List<ManagerServer.Model.Object>();

            foreach (var e in salesInvoices.GroupBy(x => x.Item1))
            {
                if (e.All(x => !x.Item3)) continue;

                foreach (var e2 in e)
                {
                    e2.Item2.DiscountAmount = e2.Item4;
                }

                var salesInvoice = ProtoBuf.Serializer.DeepClone(e.Key);
                salesInvoice.DiscountType = ManagerServer.Model.Enums.DiscountType.ExactAmount;
                salesInvoice.Key = e.Key.Key;

                objectsToUpdate.Add(salesInvoice);
            }

            ApplicationData.Businesses.Process(Business, objectsToUpdate.ToArray(), "Administrator");

            Write("OK - "+objectsToUpdate.Count.ToString());
        }
    }
}

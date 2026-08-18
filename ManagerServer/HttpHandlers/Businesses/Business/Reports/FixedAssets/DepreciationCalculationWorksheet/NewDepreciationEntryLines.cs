using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.NewDepreciationEntry))]
    [Guide("The New Depreciation Entry Lines screen creates depreciation entries from the worksheet.")]
    [Guide("Review and create journal entries for calculated depreciation differences.")]
    [Columns]
    internal sealed class NewDepreciationEntryLines : NakedObjectsWithCustomFields<ManagerServer.Model.DepreciationEntry.Line>
    {
        [ProtoMember(1)] public DateTime FromDate;
        [ProtoMember(2)] public DateTime ToDate;

        protected override void InnerGet4(Context context)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            var depreciationEntryLines = new List<ManagerServer.Model.DepreciationEntry.Line>();

            var fixedAssets = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.FixedAsset>().OrderBy(x => x.NameWithCode);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Date <= ToDate && x.FixedAsset != null).ToArray();

            var daysWithinPeriod = 0;
            if (ToDate >= FromDate)
            {
                daysWithinPeriod = (int)Math.Floor((ToDate - FromDate).TotalDays) + 1;
            }

            foreach (var e in fixedAssets)
            {
                if (e.DisposedFixedAsset && e.DisposalDate.HasValue && e.DisposalDate.Value < FromDate) continue;

                var fixedAssetTransactions = transactions.Where(x => x.FixedAsset.Key == e.Key).ToArray();
                var depreciationEntries = fixedAssetTransactions
                    .Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation)
                    .Where(x => x.Date >= FromDate && x.Date <= ToDate)
                    .Sum(x => x.BaseAmount) * -1m;

                var bookValue = fixedAssetTransactions
                    .Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets || x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation)
                    .Where(x => x.Date < FromDate)
                    .Sum(x => x.BaseAmount);

                var recalculatedDepreciation = 0m;
                var lastDate = FromDate;
                foreach (var e2 in fixedAssetTransactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets && x.Date >= FromDate).GroupBy(x => x.Date).OrderBy(x => x.Key))
                {
                    var amount = e2.Sum(x => x.BaseAmount);

                    if (bookValue > 0m)
                    {
                        var depreciationDays = (int)(e2.Key - lastDate).TotalDays;
                        var depreciation = baseCurrency.Round(bookValue / 100m * e.DepreciationRate * (depreciationDays / 365m));
                        recalculatedDepreciation += depreciation;
                    }

                    lastDate = e2.Key;
                    bookValue += amount;
                    bookValue -= recalculatedDepreciation;
                }

                if (bookValue > 0m)
                {
                    var depreciationDays = (int)(ToDate.AddDays(1) - lastDate).TotalDays;
                    var depreciation = baseCurrency.Round(bookValue / 100m * e.DepreciationRate * (depreciationDays / 365m));

                    recalculatedDepreciation += depreciation;
                }

                var difference = recalculatedDepreciation - depreciationEntries;

                if (difference != 0m)
                {
                    depreciationEntryLines.Add(new DepreciationEntry.Line() { FixedAsset = e.Key, Amount = difference });
                }
            }

            context.Set<Array>(depreciationEntryLines.ToArray());
            context.Set(new BatchOperation()
            {
                Name = Strings.Create
            });

            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(ManagerServer.Model.DepreciationEntry.Line[] rows)
        {
            var list = new List<Tuple<string, byte[]>>();
            foreach (var e in rows)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, e);
                    list.Add(new Tuple<string, byte[]>(nameof(NewDepreciationEntryLines), ms.ToArray()));
                }
            }
            return list.ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guide("The date when the depreciation will be recorded in the general ledger. This is typically the last day of the accounting period.")]
        [Guide("This date determines which accounting period the depreciation expense affects. Use month-end or year-end dates for consistent depreciation recording.")]
        public DateTime[] GetDate(ManagerServer.Model.DepreciationEntry.Line[] lines)
        {
            return lines.Select(x => ToDate).ToArray();
        }

        [Default]
        [Guide("The specific fixed asset for which depreciation is being calculated and recorded. Each asset depreciates based on its individual depreciation rate and method.")]
        [Guide("The system calculates depreciation for all active fixed assets that have a depreciable value. Disposed assets are excluded from calculations after their disposal date.")]
        public NamedObject[] GetFixedAsset(ManagerServer.Model.DepreciationEntry.Line[] lines)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return lines.Select(x => database.SingleOrDefault<FixedAsset>(x.FixedAsset)).ToArray();
        }

        [Default]
        [Right, Bold]
        [Guide("The calculated depreciation expense amount for this asset during the specified period. This represents the reduction in asset value due to usage and time.")]
        [Guide("This amount will be recorded as depreciation expense (debit) and accumulated depreciation (credit). The calculation is based on the asset's book value, depreciation rate, and time period.")]
        public decimal[] GetAmount(ManagerServer.Model.DepreciationEntry.Line[] lines)
        {
            return lines.Select(x => x.Amount).ToArray();
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey(nameof(NewDepreciationEntryLines)))
                {
                    var lines = form[nameof(NewDepreciationEntryLines)].ToString();
                    if (!string.IsNullOrWhiteSpace(lines))
                    {
                        var depreciationEntryLines = new List<DepreciationEntry.Line>();

                        var items = lines.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        foreach (var e in items)
                        {
                            using (var ms = new System.IO.MemoryStream(e))
                            {
                                var e2 = ProtoBuf.Serializer.Deserialize<DepreciationEntry.Line>(ms);

                                depreciationEntryLines.Add(e2);
                            }
                        }

                        if (depreciationEntryLines.Any())
                        {
                            var depreciationEntry = new DepreciationEntry();
                            depreciationEntry.Date = ToDate;
                            depreciationEntry.Description = string.Format(Strings.For_the_period_from_XXX_to_XXX, FromDate.ToLocalShortDisplayString(), ToDate.ToLocalShortDisplayString());
                            depreciationEntry.Lines = depreciationEntryLines.ToArray();

                            ApplicationData.Businesses.Process(Business, depreciationEntry, GetUserName());

                            Response.Redirect(new DepreciationEntries.DepreciationEntryView() { Business = Business, Key = depreciationEntry.Key, Referrer = this.ToUrl() }.ToUrl());
                            return;
                        }
                    }
                }
            }
        }
    }
}
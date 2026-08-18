using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForNegativeInventoryClearing : BaseGeneralLedgerTransactionsForInventoryItems
    {
        protected override void OnAfterHeader(Context context)
        {
            var unitCostColumn = context.Get<Column[]>().SingleOrDefault(x => x.Key == new Guid("847d6106-228a-46c1-9612-44a20edde391"));
            if (unitCostColumn != null)
            {
                unitCostColumn.Action = new Tuple<string, HttpHandler, bool>(Strings.Recalculate, new Settings.InventoryUnitCosts.InventoryCostCorrection() { Business = Business, ToDate = To, Referrer = this.ToUrl() }, false);
            }

            base.OnAfterHeader(context);
        }

        protected override void InnerGet4(Context context)
        {
            if (GeneralLedgerAccount == ApplicationData.Businesses.Get(Business).Single<BalanceSheetNegativeInventoryClearing>().Key)
            {
                var list = new List<InventoryItemBalances>();

                foreach (var e in GetGeneralLedgerTransactions().GroupBy(x => x.InventoryItem).OrderBy(x => x.Key.NameWithCode))
                {
                    var qty = e.Sum(x => x.Qty ?? 0m);
                    var balance = e.Sum(x => x.BaseAmount);

                    var item = new InventoryItemBalances()
                    {
                        InventoryItem = e.Key,
                        Qty = qty,
                        Balance = balance,
                        IsInactive = e.Key.Inactive
                    };

                    list.Add(item);
                }

                context.Set<Array>(list.ToArray());
            }

            base.InnerGet4(context);
        }

        protected override void OnColumnHeaderCell(Column column)
        {
            if (column.Key == new Guid("f86a19ae-65b9-40b6-91b2-1beed9dcf4b4"))
            {
                InputCheckbox(onClick: "this.form.querySelectorAll('input[type=checkbox]').forEach(x => x.checked = this.checked)", @class: "form-check-input");
            }
            else
            {
                base.OnColumnHeaderCell(column);
            }
        }

        [Default]
        [MinWidth, Center]
        [Guid("f86a19ae-65b9-40b6-91b2-1beed9dcf4b4")]
        public Tuple<string, byte[]>[] GetCheckbox(InventoryItemBalances[] rows)
        {
            var output = new Tuple<string, byte[]>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];

                using (var ms = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, new ManagerServer.Model.JournalEntry.Line()
                    {
                        Item = row.InventoryItem.Key,
                        Qty = -row.Qty,
                        Debit = row.Balance < 0 ? -row.Balance : 0m,
                        Credit = row.Balance > 0 ? row.Balance : 0m
                    });
                    output[i] = new Tuple<string, byte[]>(nameof(BaseGeneralLedgerTransactionsForNegativeInventoryClearing), ms.ToArray());
                }
            }

            return output;
        }

        [Default]
        [Guid("098d8348-792e-46d6-a4d2-aeffb8595b0a")]
        public NamedObject[] GetInventoryItem(InventoryItemBalances[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default, Center, MinWidth, WhitespaceNoWrap]
        [Guid("1611144e-0b55-4eb8-bdc7-a4d24cf0f6ac")]
        public Tuple<decimal, BusinessTemplate>[] GetQty(InventoryItemBalances[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Qty, GetHttpHandlerWithInventoryItemsQty(x.InventoryItem, To, referrer))).ToArray();
        }

        [Default, Bold, Right, WhitespaceNoWrap, Sum]
        [Guid("847d6106-228a-46c1-9612-44a20edde391")]
        public Tuple<decimal, BusinessTemplate>[] GetBalance(InventoryItemBalances[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Balance, GetHttpHandlerWithInventoryItemsCost(x.InventoryItem, To, referrer))).ToArray();
        }

        private BusinessTemplate GetHttpHandlerWithInventoryItemsQty(InventoryItem inventoryItem, DateTime date, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsForSubaccount;
            businessTemplate.From = null;
            businessTemplate.To = date;
            businessTemplate.SkipRevaluation = true;
            businessTemplate.GeneralLedgerAccount = ApplicationData.Businesses.Get(Business).Single<BalanceSheetInventoryOnHandAccount>().Key;
            businessTemplate.InventoryItemQty = inventoryItem.Key;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            businessTemplate.Skip = 0;
            return businessTemplate;
        }

        private BusinessTemplate GetHttpHandlerWithInventoryItemsCost(InventoryItem inventoryItem, DateTime date, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsForSubaccount;
            businessTemplate.From = null;
            businessTemplate.To = date;
            businessTemplate.SkipRevaluation = true;
            businessTemplate.GeneralLedgerAccount = ApplicationData.Businesses.Get(Business).Single<BalanceSheetInventoryOnHandAccount>().Key;
            businessTemplate.InventoryItemCost = inventoryItem.Key;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            businessTemplate.Skip = 0;
            return businessTemplate;
        }

        public sealed class InventoryItemBalances : IsInactive
        {
            public InventoryItem InventoryItem;
            public decimal Qty;
            public decimal Balance;
            public bool IsInactive;

            bool IsInactive.IsInactive => IsInactive;
        }

        protected override void OnBeforeFooter(Context context)
        {
            if (context.Get<Array>() is InventoryItemBalances[])
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex items-center gap-3"))
                    {
                        I(@class: "fas fa-fw fa-turn-up fa-rotate-90", style: "font-size: 32px; color: #ccc");

                        var buttonClass = "btn btn-primary";
                        using (Button(@class: buttonClass)) using (Span(@class: "font-semibold")) Write(Strings.NewJournalEntry);
                    }
                }
            }

            base.OnBeforeFooter(context);
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey(nameof(BaseGeneralLedgerTransactionsForNegativeInventoryClearing)))
                {
                    var item = form[nameof(BaseGeneralLedgerTransactionsForNegativeInventoryClearing)].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var items = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        var journalEntry = new JournalEntry();
                        journalEntry.Date = new[] { DateTime.Today, To }.Min();

                        journalEntry.Narration = Strings.NegativeInventoryClearing;
                        journalEntry.ItemColumn = true;
                        journalEntry.QuantityColumn = true;

                        var lines = new List<JournalEntry.Line>();
                        foreach (var e in items)
                        {
                            using (var ms = new MemoryStream(e))
                            {
                                var line = ProtoBuf.Serializer.Deserialize<JournalEntry.Line>(ms);
                                lines.Add(line);
                            }
                        }

                        var adjustment = -lines.Sum(x => (x.GetDebit() ?? 0m) - (x.GetCredit() ?? 0m));

                        lines.Add(new JournalEntry.Line()
                        {
                            Account = ApplicationData.Businesses.Get(Business).Single<ProfitAndLossStatementAccountInventoryPurchases>().Key,
                            Debit = adjustment > 0m ? adjustment : 0m,
                            Credit = adjustment < 0m ? -adjustment : 0m
                        });

                        journalEntry.Lines = lines.ToArray();

                        ApplicationData.Businesses.Process(Business, journalEntry, GetUserName());
                        Response.Redirect(new JournalEntries.JournalEntryView() { Business = Business, Key = journalEntry.Key, Referrer = this.ToUrl() }.ToUrl());
                        return;
                    }
                }
            }
            await base.InnerPost();
        }
    }
}
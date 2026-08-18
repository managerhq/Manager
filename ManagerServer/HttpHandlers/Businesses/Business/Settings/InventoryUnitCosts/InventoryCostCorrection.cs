using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryCostCorrection))]
    [Guide("The **Inventory Cost Correction** screen calculates what your *inventory unit costs* should be, compares them to what they currently are, and suggests necessary changes.")]
    [Guide("This tool ensures your inventory costs remain accurate by analyzing your transaction history and proposing corrections when discrepancies are found.")]
    [Header("Accessing Inventory Cost Correction")]
    [Guide("To access the **Inventory Cost Correction** screen, go to the **Settings** tab, then click on **Inventory Unit Costs**.")]
    [SettingsItemScreenshot("fa-scanner-keyboard", nameof(Strings.InventoryUnitCosts))]
    [Guide("Then click the **Inventory Cost Correction** button in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(nameof(Strings.InventoryCostCorrection))]
    [Header("Using Inventory Cost Correction")]
    [Guide("To process inventory cost corrections, first click the **Recalculate** button. This will recalculate *inventory unit costs* based on your past inventory transactions.")]
    [Guide("When recalculation is complete, the next screen will show how many *inventory unit costs* need to be created, updated, or deleted.")]
    [Guide("You can review these changes in detail by expanding the worksheet to see individual cost adjustments for each *inventory item*.")]
    [Guide("To accept these changes, click the **Apply Changes** button. This will update your *inventory unit costs* to match the calculated values.")]
    [PrimaryButtonScreenshot(nameof(Strings.ApplyChanges))]
    [Header("Lock Date Protection")]
    [Guide("The **Inventory Cost Correction** screen respects your **Lock Date** settings. It will not propose changes to *inventory unit costs* for locked periods.")]
    [Guide("This prevents unintended changes to your historical balances, ensuring that closed accounting periods remain unchanged.")]
    [LinkGuide("For more information about lock dates, see:", typeof(LockDate.LockDateForm))]
    [Header("Why Manual Correction is Necessary")]
    [Guide("You might wonder why Manager doesn't automatically recalculate inventory costs when transactions change. There are several important reasons for requiring manual correction:")]
    [Guide("**Performance considerations**: Automatic recalculation would slow down Manager when historical transactions are created, updated, or deleted. The system would need to recalculate costs for all subsequent transactions, which could be time-consuming for businesses with many inventory items.")]
    [Guide("**Production order complexity**: If your business uses *production orders*, recalculating costs for one *inventory item* can affect costs of other items due to manufacturing processes. This creates a cascading effect that requires extensive recalculation across multiple items and periods.")]
    [Guide("**Predictable adjustments**: When making historical adjustments, you often want account balances to change in predictable ways. Automatic full inventory recalculation might produce unexpected results.")]
    [Guide("**Negative inventory situations**: When you sell *inventory items* before purchasing or manufacturing them, the true cost isn't known until later. This means purchases or *production orders* would need to retroactively update historical costs, which can be complex to manage automatically.")]
    [Guide("**Control over historical data**: You might want to limit how far back Manager recalculates inventory costs to preserve the integrity of closed periods. This control is maintained through your *lock date* configuration.")]
    [Guide("The **Inventory Cost Correction** screen gives you a faster, more predictable system with greater control. You can periodically recalculate inventory costs while maintaining full control over which periods are affected, ensuring that closed historical figures don't accidentally change.")]
    internal sealed class InventoryCostCorrection : BusinessTemplate
    {
        [ProtoMember(2)] public DateTime? ToDate;
        [ProtoMember(3)] public bool Ajax;

        protected override void InnerGet2()
        {
            if (!Ajax)
            {
                using (Div(hxGet: new InventoryCostCorrection() { Business = Business, ToDate = ToDate, Ajax = true, Referrer = Referrer }.ToUrl(), hxTrigger: "load", hxSelect: "#card-ajax"))
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-header"))
                        {
                            using (Div(@class: "card-title")) Write(Strings.InventoryCostCorrection);
                        }
                        using (Div(@class: "card-inset"))
                        {
                            using (Div(@class: "p-4"))
                            {
                                I(@class: "fas fa-circle-notch fa-spin opacity-25 text-3xl");
                            }
                        }
                    }
                }
                return;
            }

            var database = ApplicationData.Businesses.Get(Business);

            var fromDate = DateTime.MinValue;

            var unlockedDate = database.Single<ManagerServer.Model.LockDate>().GetUnlockedDate();
            if (fromDate < unlockedDate)
            {
                fromDate = unlockedDate;
            }

            var recalculatedInventoryUnitCosts = new ManagerServer.Api.Businesses.Business.Settings.InventoryUnitCosts.GetRecalculatedInventoryUnitCosts() { Business = Business, FromDate = fromDate, Context = HttpContext }.AuthorizedHandle();
            var currentInventoryUnitCosts = database.OfType<InventoryUnitCost>().Where(x => x.InventoryItem.HasValue).Where(x => x.Date >= fromDate).ToArray();

            if (ToDate.HasValue)
            {
                recalculatedInventoryUnitCosts = recalculatedInventoryUnitCosts.Where(x => x.Date <= ToDate.Value).ToArray();
                currentInventoryUnitCosts = currentInventoryUnitCosts.Where(x => x.Date <= ToDate.Value).ToArray();
            }

            var list = new List<Item>();

            foreach (var e in currentInventoryUnitCosts.ExceptBy(recalculatedInventoryUnitCosts.Select(x => (x.InventoryItem.Value, x.Date, x.UnitCost)), x => (x.InventoryItem.Value, x.Date, x.UnitCost)))
            {
                list.Add(new Item()
                {
                    Key = e.Key,
                    InventoryItem = e.InventoryItem.Value,
                    Date = e.Date,
                    CurrentUnitCost = e.UnitCost,
                    Action = ItemAction.Delete
                });
            }

            foreach (var e in recalculatedInventoryUnitCosts.ExceptBy(currentInventoryUnitCosts.Select(x => (x.InventoryItem.Value, x.Date, x.UnitCost)), x => (x.InventoryItem.Value, x.Date, x.UnitCost)))
            {
                list.Add(new Item()
                {
                    InventoryItem = e.InventoryItem.Value,
                    Date = e.Date,
                    NewUnitCost = e.UnitCost,
                    Action = ItemAction.Create
                });
            }

            foreach (var e in list.GroupBy(x => (x.Date, x.InventoryItem)).Where(x => x.Count() > 1).Select(x => x.ToArray()))
            {
                var deleteObject = e.FirstOrDefault(x => x.Action == ItemAction.Delete);
                var createObject = e.FirstOrDefault(x => x.Action == ItemAction.Create);

                if (deleteObject != null && createObject != null)
                {
                    createObject.Action = ItemAction.Update;
                    createObject.Key = deleteObject.Key;
                    createObject.CurrentUnitCost = deleteObject.CurrentUnitCost;
                    list.Remove(deleteObject);
                }
            }

            using (Script())
            {
                Write(@"function toggleColumn(headerCb) {
    const th = headerCb.closest('th,td');
    if (!th) return;
    const table = th.closest('table');
    if (!table) return;

    // figure out logical column index
    const row = th.parentElement;
    let targetIdx = 0, acc = 0;
    for (const c of row.children) {
        if (c === th) { targetIdx = acc; break; }
        acc += c.colSpan || 1;
    }

    // loop rows, find matching cell, toggle checkboxes
    table.querySelectorAll('tr').forEach(r => {
        let col = 0;
        for (const cell of r.children) {
            const span = cell.colSpan || 1;
            if (col <= targetIdx && targetIdx < col + span) {
                if (cell !== th) {
                    cell.querySelectorAll('input[type=checkbox]').forEach(cb => {
                        if (!cb.disabled) {
                            cb.checked = headerCb.checked;
                            cb.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                    });
                }
                break;
            }
            col += span;
        }
    });
}");
            }

            using (Div(@class: "card", id: "card-ajax"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(Strings.InventoryCostCorrection);
                }
                using (Details())
                {
                    using (Summary(@class: "card-header cursor-pointer list-item"))
                    {
                        using (Span(@class: "font-semibold")) Write(Strings.InventoryUnitCosts);
                        using (Span(@class: "mx-2 bg-[var(--input)] border border-[var(--input-border)] text-[var(--input-foreground)]/60 text-xs whitespace-nowrap py-0 px-2 rounded tabular-nums font-semibold"))
                        {
                            Write(list.Count.ToString());
                        }
                    }
                    if (list.Count == 0)
                    {
                        using (Div(@class: "card-inset p-24 text-center"))
                        {
                            using (Span(@class: "card-title text-xl")) Write(Strings.Empty);
                        }
                    }
                    else
                    {
                        using (Div(@class: "card-container"))
                        {
                            foreach (var e in list.GroupBy(x => x.InventoryItem))
                            {
                                var inventoryItem = database.SingleOrDefault<InventoryItem>(e.Key);

                                using (Details())
                                {
                                    using (Summary(@class: "card-header cursor-pointer list-item"))
                                    {
                                        using (Span()) Write(inventoryItem?.NameWithCode);
                                        using (Span(@class: "mx-2 bg-[var(--input)] border border-[var(--input-border)] text-[var(--input-foreground)]/60 text-xs whitespace-nowrap py-0 px-2 rounded tabular-nums"))
                                        {
                                            Write(e.Count().ToString());

                                        }
                                    }
                                    using (Div(@class: "card-container"))
                                    {
                                        using (Table(@class: "card-table"))
                                        {
                                            using (THead())
                                            {
                                                using (Tr())
                                                {
                                                    using (Th(@class: "w-px")) InputCheckbox(@class: "form-check-input", @checked: true, onClick: "toggleColumn(this)");
                                                    using (Th(@class: "w-px whitespace-nowrap text-center")) Write(Strings.Date);
                                                    using (Th()) Write(Strings.InventoryItem);
                                                    using (Th(@class: "text-right")) Write(Strings.UnitCost);
                                                    using (Th(@class: "w-px whitespace-nowrap text-center")) Write(Strings.Action);
                                                }
                                            }
                                            using (TBody())
                                            {
                                                foreach (var e2 in e.OrderBy(x => x.Date))
                                                {
                                                    using (Tr())
                                                    {
                                                        using (Td(@class: "w-px"))
                                                        {
                                                            var itemData = new[]
                                                            {
                                                                Tuple.Create("action", ((int)e2.Action).ToString(CultureInfo.InvariantCulture)),
                                                                Tuple.Create("key", e2.Key.ToString()),
                                                                Tuple.Create("inventory-item", e2.InventoryItem.ToString()),
                                                                Tuple.Create("date", e2.Date.ToString("o", CultureInfo.InvariantCulture)),
                                                                Tuple.Create("unit-cost", e2.NewUnitCost.ToString(CultureInfo.InvariantCulture))
                                                            };
                                                            InputCheckbox(@class: "form-check-input js-icc-item", @checked: true, data: itemData);
                                                        }
                                                        using (Td(@class: "w-px whitespace-nowrap text-center")) Write(e2.Date.ToLocalShortDisplayString());
                                                        using (Td()) Write(inventoryItem?.NameWithCode);
                                                        using (Td(@class: "font-semibold text-right"))
                                                        {
                                                            if (e2.Action == ItemAction.Update || e2.Action == ItemAction.Create)
                                                            {
                                                                using (Span()) Write(e2.NewUnitCost.ToNumberString());
                                                            }
                                                            if (e2.Action == ItemAction.Update)
                                                            {
                                                                Write("&nbsp;&nbsp;");
                                                            }
                                                            if (e2.Action == ItemAction.Update || e2.Action == ItemAction.Delete)
                                                            {
                                                                using (Span(@class: "text-red-500 line-through")) Write(e2.CurrentUnitCost.ToNumberString());
                                                            }
                                                        }
                                                        using (Td(@class: "w-px whitespace-nowrap text-center"))
                                                        {
                                                            if (e2.Action == ItemAction.Create)
                                                            {
                                                                using (Div(@class: "inline-block text-white rounded font-semibold px-2.5 py-0.5 rounded bg-blue-500"))
                                                                {
                                                                    Write(Strings.Create);
                                                                }
                                                            }
                                                            if (e2.Action == ItemAction.Update)
                                                            {
                                                                using (Div(@class: "inline-block text-white rounded font-semibold px-2.5 py-0.5 rounded bg-green-500"))
                                                                {
                                                                    Write(Strings.Update);
                                                                }
                                                            }
                                                            if (e2.Action == ItemAction.Delete)
                                                            {
                                                                using (Div(@class: "inline-block text-white rounded font-semibold px-2.5 py-0.5 rounded bg-red-500"))
                                                                {
                                                                    Write(Strings.Delete);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (list.Count == 0)
                {
                    using (Div(@class: "card-header"))
                    {
                        Write("Inventory cost correction ran successfully and found zero inventory unit costs to adjust.");
                    }
                }

                using (Div(@class: "card-header"))
                {
                    if (list.Count == 0)
                    {
                        using (Button(@class: "btn btn-primary", disabled: true))
                        {
                            Write(Strings.ApplyChanges);
                        }
                    }
                    else
                    {
                        using (Button(@class: "btn btn-primary", type: "button", onclick: "applyInventoryCostCorrection(this)"))
                        {
                            Write(Strings.ApplyChanges);
                        }

                        using (Script())
                        {
                            Write("const __iccBusiness = " + Business.EncodeJsString() + ";");
                            Write("const __iccRedirect = " + Referrer.EncodeJsString() + ";");
                            Write(@"async function applyInventoryCostCorrection(btn) {
    btn.disabled = true;
    const creates = [], puts = [];
    document.querySelectorAll('input.js-icc-item:checked').forEach(cb => {
        const d = cb.dataset;
        if (d.action === '0') {
            creates.push({ date: d.date, inventoryItem: d.inventoryItem, unitCost: Number(d.unitCost) });
        } else if (d.action === '1') {
            puts.push({ key: d.key, value: { date: d.date, inventoryItem: d.inventoryItem, unitCost: Number(d.unitCost) } });
        } else if (d.action === '2') {
            puts.push({ key: d.key, value: null });
        }
    });
    const url = '/api4/inventory-unit-cost-batch';
    const jsonHeaders = { 'Content-Type': 'application/json' };
    async function send(method, body) {
        const res = await fetch(url, { method, headers: jsonHeaders, body: JSON.stringify(body) });
        if (!res.ok) throw new Error(method + ' ' + url + ' failed: ' + res.status + ' ' + await res.text());
    }
    try {
        if (creates.length) await send('POST', { business: __iccBusiness, values: creates });
        if (puts.length) await send('PUT', { business: __iccBusiness, values: puts });
        window.location.href = __iccRedirect;
    } catch (e) {
        btn.disabled = false;
        alert(e.message);
    }
}");
                        }
                    }
                }
            }
        }

        public sealed class Item
        {
            public Guid Key;
            public DateTime Date;
            public Guid InventoryItem;
            public decimal CurrentUnitCost;
            public decimal NewUnitCost;
            public ItemAction Action;
        }

        public enum ItemAction
        {
            [Primary] Create = 0,
            [Success] Update = 1,
            [Danger] Delete = 2
        }

    }
}
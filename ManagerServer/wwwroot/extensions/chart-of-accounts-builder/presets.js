// Predefined profit & loss chart-of-accounts structures offered by the builder.
//
//   group.type    : 0 = Income group, 1 = Expense group, 2 = Subgroup (of group.parent)
//                   (matches Model.Enums.ProfitAndLossStatementGroupType)
//   account.group : the id of the preset group the account is filed under.
//   sequence      : the top-level profit & loss order. Each entry is either a
//                   `group` (a top-level group from `groups`) or a `subtotal`
//                   (a running total of everything above it). The array index
//                   becomes the Position of that group / subtotal, so groups
//                   and subtotals share one ordering.
//
// A subtotal sums every line from the top of the statement down to it, so the
// groups it should span must be top-level — not subgroups. The final
// "Net profit (loss)" total is built into every profit & loss statement and is
// not created here.
//
// Display order of subgroups within a group, and of accounts within a group,
// is the array order — the builder turns that into each object's Position.
const CHART_PRESETS = {
  simple: {
    label: "Simple",
    description: "Two top-level groups — Income and Expenses — with a flat list of common accounts under each. Best for sole traders and small businesses.",
    sequence: [
      { group: "income" },
      { group: "expenses" },
    ],
    groups: [
      { id: "income",   name: "Income",   type: 0 },
      { id: "expenses", name: "Expenses", type: 1 },
    ],
    // Depreciation and inventory cost accounts are intentionally omitted —
    // Manager creates those as control accounts via its Fixed Assets and
    // Inventory features.
    accounts: [
      { name: "Sales",                   group: "income" },
      { name: "Other income",            group: "income" },
      { name: "Wages & salaries",        group: "expenses" },
      { name: "Rent",                    group: "expenses" },
      { name: "Utilities",               group: "expenses" },
      { name: "Office expenses",         group: "expenses" },
      { name: "Motor vehicle expenses",  group: "expenses" },
      { name: "Bank charges",            group: "expenses" },
      { name: "Other expenses",          group: "expenses" },
    ],
  },
  detailed: {
    label: "Detailed",
    description: "Separate groups for Income, Cost of sales, Operating expenses, Other income and Finance costs — with Gross profit and Operating profit subtotals, and expense subgroups — for a full profit & loss statement.",
    sequence: [
      { group: "income" },
      { group: "cost-of-sales" },
      { subtotal: "Gross profit (loss)" },
      { group: "operating-expenses" },
      { subtotal: "Operating profit (loss)" },
      { group: "other-income" },
      { group: "finance-costs" },
    ],
    groups: [
      { id: "income",             name: "Income",                   type: 0 },
      { id: "cost-of-sales",      name: "Cost of sales",            type: 1 },
      { id: "operating-expenses", name: "Operating expenses",       type: 1 },
      { id: "other-income",       name: "Other income",             type: 0 },
      { id: "finance-costs",      name: "Finance costs",            type: 1 },
      // Subgroups nest inside Operating expenses.
      { id: "selling", name: "Selling & distribution",   type: 2, parent: "operating-expenses" },
      { id: "admin",   name: "General & administration", type: 2, parent: "operating-expenses" },
    ],
    // Depreciation and inventory cost accounts (and asset-disposal gains/losses)
    // are intentionally omitted — Manager creates those as control accounts via
    // its Fixed Assets and Inventory features.
    accounts: [
      { name: "Sales",                   group: "income" },
      { name: "Other revenue",           group: "income" },
      { name: "Freight & courier",       group: "cost-of-sales" },
      { name: "Direct labour",           group: "cost-of-sales" },
      { name: "Advertising & marketing", group: "selling" },
      { name: "Entertainment",           group: "selling" },
      { name: "Travel & accommodation",  group: "selling" },
      { name: "Motor vehicle expenses",  group: "selling" },
      { name: "Wages & salaries",        group: "admin" },
      { name: "Rent",                    group: "admin" },
      { name: "Utilities",               group: "admin" },
      { name: "Insurance",               group: "admin" },
      { name: "Office supplies",         group: "admin" },
      { name: "Telephone & internet",    group: "admin" },
      { name: "Repairs & maintenance",   group: "admin" },
      { name: "Accounting & legal fees", group: "admin" },
      { name: "Interest income",         group: "other-income" },
      { name: "Bank charges",            group: "finance-costs" },
      { name: "Interest expense",        group: "finance-costs" },
    ],
  },
};

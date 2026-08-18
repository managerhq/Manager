using System.Collections.Generic;
using System.Linq;
using ManagerServer.Helpers;
using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Model.Master;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    [ProtoContract]
    [Title(nameof(Strings.Summary))]
    [Guide("The **Summary** tab shows the balances of various accounts, offering a swift overview of the financial wellbeing of your business.")]
    [TabScreenshot(icon: "fa-presentation", name: nameof(Strings.Summary))]
    [Guide("This encompasses details on assets, liabilities, equity, income, and expenses, all organized into distinct accounts or categories for straightforward navigation.")]
    [Guide("It serves as a dashboard, allowing users to quickly check the current financial condition of their business.")]
    [Header("Setting the Accounting Period")]
    [Guide("By default, the *Summary* tab shows balances for all entered transactions. This is appropriate if starting a new business on Manager.io.")]
    [Guide("However once you use Manager for more than one accounting period, you want to tailor your *Summary* screen so it shows balances for your current accounting period only.")]
    [Guide("Click the **Edit** button to set the period for your *Summary* tab and other parameters relevant to your specific business situation.")]
    [HeroButtonScreenshot(title: nameof(Strings.Summary), name: nameof(Strings.Edit))]
    [LinkGuide("For more information, see:", typeof(SummaryForm))]
    [Header("Customizing the Layout")]
    [Guide("The layout of groups, accounts, and totals on the *Summary* tab can be adjusted through the **Chart of Accounts**.")]
    [Guide("This feature helps in organizing your financial information in a way that best suits your business operations.")]
    [SettingsItemScreenshot("fa-sitemap", nameof(Strings.ChartOfAccounts))]
    [LinkGuide("For more information, see:", typeof(Settings.ChartOfAccounts.ChartOfAccounts))]
    [Header("Viewing Transaction Details")]
    [Guide("The *Summary* tab shows balances for all your balance sheet and profit and loss statement accounts.")]
    [Guide("However, you can also view all individual transactions making up your balances on the *Summary* tab by clicking the **Transactions** button in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(nameof(Strings.Transactions))]
    [LinkGuide("For more information, see:", typeof(Transactions))]
    internal sealed class SummaryView : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var referrer = this.ToUrl();

            var summary = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.Summary>();

            var accountingBasis = ManagerServer.Model.Enums.AccountingBasis.AccrualBasis;
            if (summary.ShowBalancesOnCashBasis) accountingBasis = ManagerServer.Model.Enums.AccountingBasis.CashBasis;

            var accountCodes = summary.AccountCodes;            

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            transactions = transactions.DisposeFixedAssets();
            transactions = transactions.DisposeIntangibleAssets();

            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;

            var database = ApplicationData.Businesses.Get(Business);

            if (summary.ShowBalancesForSpecifiedPeriod)
            {
                from = summary.FromDate;
                if (summary.ToDate == ManagerServer.Model.Enums.DateType.Today) to = DateTime.Today;
                if (summary.ToDate == ManagerServer.Model.Enums.DateType.Custom) to = summary.ToDateValue;
            }

            var groupsToCollapse = new Guid[0];
            if (summary.HasGroupsToCollapse && summary.GroupsToCollapse != null) groupsToCollapse = summary.GroupsToCollapse;            

            if (accountingBasis == ManagerServer.Model.Enums.AccountingBasis.CashBasis)
            {
                var dates = new List<DateTime>();
                if (from > DateTime.MinValue) dates.Add(from.AddDays(-1));
                dates.Add(to);

                transactions = transactions.AutomaticallyMatchSalesInvoices().AutomaticallyMatchPurchaseInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());
            }            

            /*
            var transactionCount = transactions.Count(x => !x.CashBasisAdjustment && x.Date > to);
            if (transactionCount > 0)
            {
                using (Div(style: "border: 1px solid #ddd; background-color: #ffffee; padding: 5px 10px; margin-bottom: 10px; font-size: 11px; border-radius: 4px; color: #444; line-height: 175%"))
                {
                    Write(string.Format(Strings.SummaryDescription, from.ToLocalShortDisplayString(), "<b>" + to.ToLocalShortDisplayString() + "</b>"));
                    Br();
                    Write(string.Format(Strings.TransactionWarning, "<b>" + transactionCount.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture) + "</b>", "<b>" + to.ToLocalShortDisplayString() + "</b>"));
                }
            }
            */

            transactions = transactions.Revaluate(from, to);
            
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();

            var aggregations = transactions.GetAggregations();
            var profitAndLossBalanace = aggregations.GetProfitAndLossAccountKeys().ToDictionary(x => x, x => aggregations.GetProfitAndLossAccountAmount(x, from, to));
            var balanceSheetBalances = aggregations.GetBalanceSheetAccountKeys().ToDictionary(x => x, x => aggregations.GetBalanceSheetAccountBalance(x, to));

            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex items-center gap-4"))
                    {
                        using (Div(@class: "flex items-center gap-3"))
                        {
                            using (Div(@class: "card-title")) Write(Strings.Summary);
                            WriteHelp();
                        }
                        using (A(new SummaryForm() { Business = Business, Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Summary)), Referrer = referrer }.ToUrl(), @class: "btn")) Write(Strings.Edit);
                    }
                }

                using (Div(@class: "card-inset grid grid-cols-1 lg:grid-cols-2 gap-6"))
                {
                    using (Div(@class: "flex flex-col space-y-4"))
                    {
                        using (Div(@class: "text-center"))
                        {
                            using (Div(@class: "card-title")) Write(Strings.BalanceSheet);
                            if (summary.ShowBalancesForSpecifiedPeriod) using (Div()) Write(string.Format(Strings.As_at_XXX, to.ToLocalShortDisplayString()));
                            if (summary.ShowBalancesOnCashBasis) using (Div()) Write(Strings.CashBasis);
                        }

                        foreach (var e in chartOfAccounts.BalanceSheet)
                        {
                            if (e.Key == Guid.Empty && !e.GetAllAccounts().Any()) continue;

                            var multiplier = 1m;
                            if (e.Key == ChartOfAccountGroups.Liabilities) multiplier = -1m;
                            if (e.Key == ChartOfAccountGroups.Equity) multiplier = -1m;

                            using (Div(@class: "card card-shadow"))
                            {
                                using (Div(@class: "card-body bg-[color-mix(in_oklab,_yellow_5%,_var(--card))] p-6"))
                                {
                                    using (Div(@class: "flex justify-between font-semibold", style: "font-size: 18px"))
                                    {
                                        using (Div()) Write(e.Name);
                                        using (Div(@class: "text-right observer:blur observer:hover:blur-none observer:hover:transition whitespace-nowrap"))
                                        {
                                            var balance = 0m;
                                            foreach (var e2 in e.GetAllAccounts())
                                            {
                                                if (balanceSheetBalances.ContainsKey(e2.Key)) balance = balance.SafeAdd(balanceSheetBalances[e2.Key] * multiplier);
                                            }
                                            using (Span(@class: "tabular-nums")) Write(balance.ToCurrencyString(baseCurrency, CurrencySymbol.Short));
                                        }
                                    }
                                }

                                if (!groupsToCollapse.Contains(e.Key))
                                {
                                    using (Div(@class: "card-body p-6"))
                                    {
                                        using (Table(@class: "w-full"))
                                        {
                                            foreach (var e2 in e.Items)
                                            {
                                                printBalanceSheetItem(e2, multiplier, to, baseCurrency, accountingBasis, 0, accountCodes, summary.ExcludeZeroBalances, referrer, balanceSheetBalances, groupsToCollapse);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    using (Div(@class: "flex flex-col space-y-4"))
                    {
                        using (Div(@class: "text-center"))
                        {
                            using (Div(@class: "card-title")) Write(Strings.ProfitAndLossStatement);
                            if (summary.ShowBalancesForSpecifiedPeriod) using (Div()) Write(string.Format(Strings.For_the_period_from_XXX_to_XXX, from.ToLocalShortDisplayString(), to.ToLocalShortDisplayString()));
                            if (summary.ShowBalancesOnCashBasis) using (Div()) Write(Strings.CashBasis);
                        }

                        var total = 0m;

                        foreach (var e in chartOfAccounts.ProfitAndLossStatement)
                        {
                            if (e.Key == Guid.Empty && !e.GetAllAccounts().Any()) continue;

                            if (e.IsSubtotal)
                            {
                                using (Div(@class: "card card-shadow"))
                                {
                                    using (Div(@class: "card-body bg-[color-mix(in_oklab,_yellow_5%,_var(--card))] p-6"))
                                    {
                                        using (Div(@class: "flex justify-between font-semibold", style: "font-size: 18px"))
                                        {
                                            using (Div()) Write(e.Name);
                                            using (Div(@class: "tabular-nums text-right observer:blur observer:hover:blur-none observer:hover:transition whitespace-nowrap")) Write(total.ToCurrencyString(baseCurrency, CurrencySymbol.Short));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (Div(@class: "card card-shadow"))
                                {
                                    using (Div(@class: "card-body bg-[color-mix(in_oklab,_yellow_5%,_var(--card))] p-6"))
                                    {
                                        using (Div(@class: "flex justify-between font-semibold", style: "font-size: 18px"))
                                        {
                                            using (Div())
                                            {
                                                if (e.IsExpenseGroup)
                                                {
                                                    using (Span(@class: "underline")) Write(Strings.Less);
                                                    Write(" ");
                                                }
                                                Write(e.Name);
                                            }
                                            using (Div(@class: "text-right observer:blur observer:hover:blur-none observer:hover:transition whitespace-nowrap"))
                                            {
                                                var balance = 0m;
                                                foreach (var e2 in e.GetAllAccounts())
                                                {
                                                    if (profitAndLossBalanace.ContainsKey(e2.Key)) balance = balance.SafeAdd(profitAndLossBalanace[e2.Key] * -1m);
                                                }
                                                total = total.SafeAdd(balance);
                                                if (e.IsExpenseGroup) balance *= -1m;
                                                using (Span(@class: "tabular-nums")) Write(balance.ToCurrencyString(baseCurrency, CurrencySymbol.Short));
                                            }
                                        }
                                    }

                                    if (!groupsToCollapse.Contains(e.Key))
                                    {
                                        using (Div(@class: "card-body p-6"))
                                        {
                                            using (Table(@class: "w-full"))
                                            {
                                                foreach (var e2 in e.Items)
                                                {
                                                    printProfitAndLossStatementItem(e2, e.IsExpenseGroup ? 1m : -1m, from, to, baseCurrency, accountingBasis, 0, accountCodes, summary.ExcludeZeroBalances, referrer, profitAndLossBalanace, groupsToCollapse);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                }

                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex justify-between"))
                    {
                        using (Div())
                        {
                        }

                        using (Div(@class: "flex items-center gap-2"))
                        {
                            var httpHandler = new Transactions();
                            httpHandler.Business = Business;
                            httpHandler.Referrer = this.ToUrl();
                            using (A(href: httpHandler.ToUrl(), @class: "btn btn-xs")) Write(Strings.Transactions);
                        }
                    }
                }
            }
        }

        private void printBalanceSheetItem(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, decimal multiplier, DateTime to, ManagerServer.Model.Currency baseCurrency, ManagerServer.Model.Enums.AccountingBasis accountingBasis, int level, bool showAccountCodes, bool excludeZeroBalances, string referrer, Dictionary<Guid, decimal> baseBalances, Guid[] groupsToCollapse)
        {
            var padding = "padding-left: ";
            if (ManagerServer.Globalization.Languages.IsRightToLeft()) padding = "padding-right: ";
            padding += (20 * level).ToString() + "px";
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var balances = new List<decimal>();
                foreach (var e2 in group.GetAllAccounts())
                {
                    if (baseBalances.ContainsKey(e2.Key)) balances.Add(baseBalances[e2.Key] * multiplier);
                }
                if ((excludeZeroBalances || item.Inactive) && balances.All(x => x == 0m)) return;

                var isCollapsed = groupsToCollapse.Contains(group.Key);

                using (Tr())
                {
                    using (Td(@class: $"p-1{(isCollapsed ? null : " font-semibold")}", style: padding))
                    {
                        Write(item.Name);
                    }
                    using (Td(style: "width: 1px", @class: $"p-1 font-semibold whitespace-nowrap text-right observer:blur-sm observer:hover:blur-none observer:hover:transition{(isCollapsed ? " text-neutral-600" : " text-neutral-300")}"))
                    {
                        using (Span(@class: "tabular-nums"))
                        {
                            var balance = balances.SafeSum();
                            if (balance == 0m) Write("-");
                            else Write(balance.ToCurrencyString(currency: baseCurrency, currencySymbol: CurrencySymbol.None));
                        }
                    }
                }

                if (isCollapsed) return;

                foreach (var e3 in group.Items)
                {
                    printBalanceSheetItem(e3, multiplier, to, baseCurrency, accountingBasis, level+1, showAccountCodes, excludeZeroBalances, referrer, baseBalances, groupsToCollapse);
                }
            }
            else if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var balance = 0m;
                if (baseBalances.ContainsKey(account.Key)) balance = baseBalances[account.Key] * multiplier;

                if (balance == 0m)
                {
                    if (account.Inactive) return;
                    if (excludeZeroBalances) return;
                    if (account.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetInterdivisionalLoan))) return;
                    if (account.Key == ManagerServer.Model.Master.AccountKeys.Suspense) return;
                    if (accountingBasis == ManagerServer.Model.Enums.AccountingBasis.CashBasis)
                    {
                        if (account.Key == ManagerServer.Model.Master.AccountKeys.AccountsPayable) return;
                        if (account.Key == ManagerServer.Model.Master.AccountKeys.AccountsReceivable) return;
                    }
                }
                using (Tr())
                {
                    using (Td(@class: "p-1", style: padding))
                    {
                        Write((showAccountCodes ? item.NameWithCode : item.Name));
                    }
                    using (Td(style: "width: 1px", @class: "p-1 font-semibold whitespace-nowrap text-right observer:blur-sm observer:hover:blur-none observer:hover:transition tabular-nums"))
                    {
                        using (A(href: new SummaryTransactions() { CashBasis = (accountingBasis == ManagerServer.Model.Enums.AccountingBasis.CashBasis), GeneralLedgerAccount = item.Key, To = to, Business = Business, Referrer = referrer }.ToUrl()))
                        {
                            if (balance == 0m) Write("-");
                            else Write(balance.ToCurrencyString(currency: baseCurrency, currencySymbol: CurrencySymbol.None));
                        }
                    }
                }
            }
        }

        private void printProfitAndLossStatementItem(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, decimal multiplier, DateTime from, DateTime to, ManagerServer.Model.Currency baseCurrency, ManagerServer.Model.Enums.AccountingBasis accountingBasis, int level, bool showAccountCodes, bool excludeZeroBalances, string referrer, Dictionary<Guid, decimal> baseBalances, Guid[] groupsToCollapse)
        {
            var padding = "padding-left: ";
            if (ManagerServer.Globalization.Languages.IsRightToLeft()) padding = "padding-right: ";
            padding += (20 * level).ToString() + "px";
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var balances = new List<decimal>();
                foreach (var e2 in group.GetAllAccounts())
                {
                    if (baseBalances.ContainsKey(e2.Key)) balances.Add(baseBalances[e2.Key] * multiplier);
                }

                if (excludeZeroBalances && balances.All(x => x == 0m)) return;

                var isCollapsed = groupsToCollapse.Contains(group.Key);

                using (Tr())
                {
                    using (Td(@class: $"p-1{(isCollapsed ? null : " font-semibold")}", style: padding))
                    {
                        Write(item.Name);
                    }
                    using (Td(style: "width: 1px", @class: $"p-1 font-semibold whitespace-nowrap text-right observer:blur-sm observer:hover:blur-none observer:hover:transition{(isCollapsed ? " text-neutral-600" : " text-neutral-300")}"))
                    {
                        using (Span(@class: "tabular-nums"))
                        {
                            var balance = balances.SafeSum();
                            if (balance == 0m) Write("-");
                            else Write(balance.ToCurrencyString(currency: baseCurrency, currencySymbol: CurrencySymbol.None));
                        }
                    }
                }

                if (isCollapsed) return;

                foreach (var e3 in group.Items)
                {
                    printProfitAndLossStatementItem(e3, multiplier, from, to, baseCurrency, accountingBasis, level + 1, showAccountCodes, excludeZeroBalances, referrer, baseBalances, groupsToCollapse);
                }
            }
            else if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var balance = 0m;
                if (baseBalances.ContainsKey(account.Key)) balance = baseBalances[account.Key] * multiplier;
                if (balance == 0m)
                {
                    if (excludeZeroBalances) return;
                }

                using (Tr())
                {
                    using (Td(@class: "p-1", style: padding))
                    {
                        Write((showAccountCodes ? item.NameWithCode : item.Name));
                    }
                    using (Td(style: "width: 1px", @class: "p-1 tabular-nums font-semibold whitespace-nowrap text-right observer:blur-sm observer:hover:blur-none observer:hover:transition"))
                    {
                        using (A(href: new SummaryTransactions() { CashBasis = (accountingBasis == ManagerServer.Model.Enums.AccountingBasis.CashBasis), GeneralLedgerAccount = item.Key, From = from, To = to, Business = Business, Referrer = referrer }.ToUrl()))
                        {
                            if (balance == 0m) Write("-");
                            else Write(balance.ToCurrencyString(currency: baseCurrency, currencySymbol: CurrencySymbol.None));
                        }
                    }
                }
            }
        }
    }
}

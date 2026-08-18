using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Projects
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("8c8fccab-6c07-4b38-a53f-b70f766b5153")]
    [Title(nameof(Strings.Projects))]
    [Guide("The `Projects` tab helps you track income, expenses, and profitability for individual contracts, customer relationships, or specific work clusters.")]
    [Header("Getting Started")]
    [Guide("To create a new project, click the `New Project` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.Projects), name: nameof(Strings.NewProject))]
    [Guide("Once your project is set up, you can link it to financial transactions. When entering income or expenses, select the appropriate project from the drop-down menu.")]
    [Header("Tracking Expenses")]
    [Guide("Purchase orders can be assigned to projects. While purchase orders are not actual costs until invoiced, they appear in the `Purchase Orders` column.")]
    [Guide("This allows you to track potential upcoming expenses and get a more accurate picture of your project's financial status.")]
    [Header("Understanding the Columns")]
    [Guide("The `Projects` tab displays several columns to help you monitor project performance:")]
    [Columns]
    internal sealed class Projects : NakedObjectsWithAutomaticRows<Project>
    {
        [Default]
        [Guid("31bfb1cc-2d42-4f1e-bc3b-da1160ef1ff5")]
        [Guide("The project name or title. Use descriptive names like 'Website Redesign for ABC Corp' or 'Q4 Marketing Campaign'.")]
        public string[] GetName(ManagerServer.Model.Project[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        private Dictionary<Project, Balance> getBalances;
        public Dictionary<Project, Balance> GetBalances(ManagerServer.Model.Project[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

                var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                    .Where(x => x.Project != null)
                    .GroupBy(x => x.Project.Key)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var purchaseOrders = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.PurchaseOrder>()
                    .Where(x => !x.Cancelled)
                    .SelectMany(x => x.GetGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)))
                    .Where(x => x.Project != null)
                    .Select(x => new { PurchaseOrder = x.PurchaseOrderAsTransaction.Key, Project = x.Project.Key, Amount = x.BaseAmount });

                var purchaseInvoices = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.PurchaseInvoice>()
                    .Where(x => x.PurchaseOrder.HasValue)
                    .SelectMany(x => x.GetGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)))
                    .Where(x => x.Project != null)
                    .Select(x => new { PurchaseOrder = x.PurchaseInvoiceAsTransaction.PurchaseOrder.Value, Project = x.Project.Key, Amount = x.BaseAmount*-1m });

                var purchaseOrdersAndInvoices = purchaseOrders.Concat(purchaseInvoices);

                var plannedCosts = purchaseOrdersAndInvoices
                    .GroupBy(x => new { x.PurchaseOrder, x.Project })
                    .Select(x => new { x.Key, Amount = x.Sum(y => y.Amount) })
                    .Where(x => x.Amount > 0)
                    .GroupBy(x => x.Key.Project)
                    .ToDictionary(x => x.Key, x => x.Sum(y => y.Amount));

                var output = new Dictionary<Project, Balance>();
                foreach (var e in rows)
                {
                    var income = 0m;
                    var incurredCosts = 0m;
                    var profit = 0m;

                    if (generalLedger.ContainsKey(e.Key))
                    {
                        income = generalLedger[e.Key].Where(x => !x.IsProjectCost).Sum(x => x.BaseAmount) * -1m;
                        incurredCosts = generalLedger[e.Key].Where(x => x.IsProjectCost).Sum(x => x.BaseAmount);
                    }

                    var purchaseOrderAmount = 0m;
                    if (plannedCosts.TryGetValue(e.Key, out var amount))
                    {
                        purchaseOrderAmount = amount;
                    }

                    profit = income - incurredCosts;

                    var revisedProfit = profit - purchaseOrderAmount;

                    output.Add(e, new Balance()
                    {
                        Income = new Tuple<decimal, Currency, BusinessTemplate>(income, baseCurrency, new ProjectIncomeTransactions() { Business = Business, Project = e.Key, Referrer = referrer }),
                        IncurredCosts = new Tuple<decimal, Currency, BusinessTemplate>(incurredCosts, baseCurrency, new ProjectIncurredCostTransactions() { Business = Business, Project = e.Key, Referrer = referrer }),
                        Profit = new Tuple<decimal, Currency, BusinessTemplate>(profit, baseCurrency, new ProjectReportView() { Business = Business, Key = e.Key, Referrer = referrer }),
                        PurchaseOrders = new Tuple<decimal, Currency, BusinessTemplate>(purchaseOrderAmount, baseCurrency, new ProjectPurchaseOrders() { Business = Business, Project = e.Key, Referrer = referrer }),
                        RevisedProfit = new Tuple<decimal, Currency>(revisedProfit, baseCurrency)
                    });
                }

                getBalances = output;
            }
            return getBalances;
        }

        [Default]
        [Right, Sum]
        [Guid("810e06e0-b418-459a-8777-42ffc2695dc1")]
        [Guide("Total income assigned to the project.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetIncome(ManagerServer.Model.Project[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].Income).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("672c501a-84aa-4462-8190-148a55d9f7bb")]
        [Guide("Total expenses allocated to the project.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetExpenses(ManagerServer.Model.Project[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].IncurredCosts).ToArray();
        }

        [Default]
        [Right, Sum, Bold]
        [Guid("a60060ad-e798-4d7a-9ce6-e2fc8aacc085")]
        [Guide("Net profit calculated by subtracting `Expenses` from `Income`. Click this figure to view a detailed `Profit and Loss Statement` for the project.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetProfit(ManagerServer.Model.Project[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].Profit).ToArray();
        }

        [Right, Sum]
        [Guid("b1062d1e-736d-4dce-90e0-5f665d17cd78")]
        [Guide("Shows expenses from uninvoiced purchase orders. Click this figure to view all purchase orders linked to the project that are pending invoice processing.")]
        [Guide("If you are not using purchase orders, you should disable this column since it will always show zero.")]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetPurchaseOrders(ManagerServer.Model.Project[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].PurchaseOrders).ToArray();
        }

        [Right, Sum, Bold]
        [Guid("f42a5e34-c8c6-4893-a1d4-9f87903ef13a")]
        [Guide("Profit adjusted by subtracting uninvoiced purchase orders. This provides a more accurate view of the project's financial status.")]
        [Guide("For example, if profit is $10,000 and uninvoiced purchase orders total $2,000, the revised profit shows $8,000.")]
        [Guide("If you are not using purchase orders, you should disable this column since your revised profit will always be the same as profit.")]
        public Tuple<decimal, Currency>[] GetRevisedProfit(ManagerServer.Model.Project[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].RevisedProfit).ToArray();
        }

        public sealed class Balance
        {
            public Tuple<decimal, Currency, BusinessTemplate> Income;
            public Tuple<decimal, Currency, BusinessTemplate> IncurredCosts;
            public Tuple<decimal, Currency, BusinessTemplate> Profit;
            public Tuple<decimal, Currency, BusinessTemplate> PurchaseOrders;
            public Tuple<decimal, Currency> RevisedProfit;
        }
    }
}

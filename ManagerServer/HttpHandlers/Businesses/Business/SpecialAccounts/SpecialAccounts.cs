using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SpecialAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("7cd70729-db59-4ae8-9e41-6a423b26ad6a")]
    [Title(nameof(Strings.SpecialAccounts))]
    [Guide("The `SpecialAccounts` tab offers a distinctive function that enhances flexibility in accounting practices. It enables businesses to set up and oversee accounts with unique properties, distinguishing them from standard accounts. Examples of such accounts include loan accounts, customer deposits, or legal retainer accounts.")]
    [TabScreenshot("fa-cubes", nameof(Strings.SpecialAccounts))]
    [Guide("To create a new special account, click the `NewSpecialAccount` button.")]
    [HeroButtonScreenshot(nameof(Strings.SpecialAccounts), nameof(Strings.NewSpecialAccount))]
    [Guide("If you have created special account that has existing balances, you can set starting balances under `Settings`, then `StartingBalances`.")]
    [LinkGuide("For more information see:", typeof(Settings.StartingBalances.SpecialAccounts.SpecialAccountStartingBalanceList))]
    [Guide("The `SpecialAccounts` tab is comprised of several columns.")]
    [Columns]
    [Guide("Click on the `EditColumns` button to select which columns you want to be displayed.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("For more information see:", typeof(NakedObjectsWithEditColumns<SpecialAccount>))]
    [Guide("Utilize `AdvancedQueries` to enhance your data analysis on this screen. For instance, if you possess various kinds of special accounts, you can form an advanced query for each kind, enabling you to filter accounts based on their specific context.")]
    [LinkGuide("For more information see:", typeof(NakedObjectsWithAdvancedQueries))]
    internal sealed class SpecialAccounts : NakedObjectsWithAutomaticRows<ManagerServer.Model.SpecialAccount>
    {
        [WarnIfNotUnique]
        [Guid("e7ea7d70-6fdc-4a50-89ba-9b73b4d50dd5")]
        [Guide("The `Code` column displays the code for the specific account.")]
        public string[] GetCode(ManagerServer.Model.SpecialAccount[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("491154b3-b74e-4e08-931e-95a567aecef2")]
        [Guide("The `Name` column displays the name of the special account.")]
        public string[] GetName(ManagerServer.Model.SpecialAccount[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("d98d297b-13f8-415e-9cbb-3905f0693c35")]
        [Guide("The `ControlAccount` column displays the name of the control account associated with a given special account. By default, all special accounts are grouped under a control account called `SpecialAccounts`. However, you have the option to create custom control accounts. This feature enables you to categorize special accounts into various control accounts on the balance sheet, enhancing organization.")]
        [LinkGuide("For more information see:", typeof(Settings.ControlAccounts.ControlAccounts))]
        public string[] GetControlAccount(ManagerServer.Model.SpecialAccount[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForSpecialAccounts>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetSpecialAccountsAccount>()).GetName()).ToArray();
        }

        [Guid("6adeee6a-b50d-43b0-9f52-3a87066a2d4c")]
        [Guide("The `Division` column displays the name of the division that the special account belongs to. If divisional accounting is not being used, this column will remain empty.")]
        [LinkGuide("For more information see:", typeof(Settings.Divisions.Divisions))]
        public string[] GetDivision(ManagerServer.Model.SpecialAccount[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.Name).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("59674113-39b6-4155-9c79-a36199ce1713")]
        [Guide("The `Balance` column reflects the net total of all debits and credits recorded in this account. By clicking on the amount, you can access a detailed view of each transaction that contributes to the overall balance.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetBalance(ManagerServer.Model.SpecialAccount[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var balances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForSpecialAccounts && x.SpecialAccount != null).GroupBy(x => x.SpecialAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount));
            var output = new List<Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>>();
            foreach (var e in rows)
            {
                var currency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(e.Currency) as ManagerServer.Model.Currency ?? baseCurrency;
                var link = new SpecialAccountTransactions() { Business = Business, SpecialAccount = e.Key, Referrer = referrer };
                if (balances.TryGetValue(e.Key, out decimal value))
                {
                    output.Add(new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(value, currency, link));
                }
                else
                {
                    output.Add(new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(0m, currency, link));
                }
            }
            return output.ToArray();
        }
    }
}

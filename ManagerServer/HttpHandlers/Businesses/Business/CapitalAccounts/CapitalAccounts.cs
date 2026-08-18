using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.CapitalAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("f3a4668c-18cb-4e36-823c-c83b6d8eb250")]
    [Title(nameof(Strings.CapitalAccounts))]
    [Guide("The **Capital Accounts** tab tracks funds contributed by and distributed to business owners or partners.")]
    [Guide("Use capital accounts to monitor owner investments, drawings, and their share of profits or losses.")]
    [TabScreenshot("fa-user-chart", nameof(Strings.CapitalAccounts))]
    [Header("Creating Capital Accounts")]
    [Guide("Click the **New Capital Account** button to create an account for each owner or partner.")]
    [HeroButtonScreenshot(nameof(Strings.CapitalAccounts), nameof(Strings.NewCapitalAccount))]
    [LinkGuide("Learn about capital account setup:", typeof(CapitalAccountForm))]
    [Header("Setting Starting Balances")]
    [Guide("For existing capital accounts with current balances, set starting balances in **Settings** → **Starting Balances**.")]
    [LinkGuide("Learn about starting balances:", typeof(Settings.StartingBalances.CapitalAccounts.CapitalAccountStartingBalanceList))]
    [Header("Understanding the Columns")]
    [Guide("The **Capital Accounts** tab displays the following information:")]
    [Columns]
    [Guide("Click **Edit Columns** to customize which columns are visible.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn about column customization:", typeof(NakedObjectsWithEditColumns<CapitalAccounts>))]
    internal sealed class CapitalAccounts : NakedObjectsWithAutomaticRows<ManagerServer.Model.CapitalAccount>
    {
        [WarnIfNotUnique]
        [Guid("205a15dc-aea6-469a-b31f-bf3d6c5a11a6")]
        [Guide("The **Code** column displays the code for the capital account.")]
        public string[] GetCode(ManagerServer.Model.CapitalAccount[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("752c3c90-4f7c-47a6-a4ca-df695de2c607")]
        [Guide("The **Name** column displays the name of the capital account.")]
        public string[] GetName(ManagerServer.Model.CapitalAccount[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("47bcda98-6b05-42ee-ab79-a4151aa5c869")]
        [Guide("The **Control Account** column shows where this capital account appears on the **Balance Sheet**.")]
        [Guide("The default is *Capital Accounts* unless you've created custom control accounts.")]
        public string[] GetControlAccount(ManagerServer.Model.CapitalAccount[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForCapitalAccounts>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetCapitalAccountsAccount>()).GetName()).ToArray();
        }

        [Guid("434099ac-0916-4462-842d-735a608baffe")]
        [Guide("The **Division** column shows the division assigned to this capital account for divisional reporting.")]
        public string[] GetDivision(ManagerServer.Model.CapitalAccount[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.Name).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("2c010ace-1027-41f0-a2df-0514f338105f")]
        [Guide("The **Balance** column shows the current balance of each capital account.")]
        [Guide("Click the balance amount to view all transactions that make up this balance.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetBalance(ManagerServer.Model.CapitalAccount[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var balances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForCapitalAccounts).GroupBy(x => x.CapitalAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
            var output = new List<Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>>();
            foreach (var e in rows)
            {
                var link = new CapitalAccoutTransactions() { Business = Business, CapitalAccount = e.Key, Referrer = referrer };
                if (balances.TryGetValue(e.Key, out decimal value))
                {
                    output.Add(new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(value, baseCurrency, link));
                }
                else
                {
                    output.Add(new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(0m, baseCurrency, link));
                }
            }
            return output.ToArray();
        }
    }
}

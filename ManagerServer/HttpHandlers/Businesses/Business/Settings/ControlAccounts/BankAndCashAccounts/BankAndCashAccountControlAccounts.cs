using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.BankAndCashAccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(BankAndCashAccounts))]
    [Title(nameof(Strings.ControlAccounts), nameof(Strings.BankAndCashAccounts))]
    [NewButton(nameof(Strings.NewControlAccount))]
    [Guide("This screen allows you to create custom *control accounts* for your bank and cash accounts.")]
    internal sealed class BankAndCashAccountControlAccounts : PersistentObjectTable<ManagerServer.Model.ControlAccountForBankAccounts>
    {
        [Guid("af178c50-88ce-4b3c-be08-53ffa604ada5")]
        public string GetName(ManagerServer.Model.ControlAccountForBankAccounts row) => row.Name;

        [Guid("4070d6ee-bed0-4108-a04a-8579d9c6b64a")]
        public BalanceSheetAbstractGroup GetGroup(ManagerServer.Model.ControlAccountForBankAccounts row)
        {
            if (!row.Group.HasValue) return null;
            return ApplicationData.Businesses.Get(Business).SingleOrDefault(row.Group.Value) as BalanceSheetAbstractGroup ?? ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
        }
    }
}

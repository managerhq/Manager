using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SpecialAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.SpecialAccount), nameof(Strings.Transactions))]
    [Guide("This screen displays all transactions posted to a *special account*. Special accounts are used to track specific business activities that require detailed monitoring outside of your normal chart of accounts.")]
    [Guide("Examples of special accounts include trust funds, escrow accounts, client money accounts, or any other funds that need to be tracked separately from your regular business operations.")]
    [Header("Understanding Transaction Details")]
    [Guide("Each transaction shows the date, description, and amount affecting this special account. Positive amounts represent money coming into the account, while negative amounts represent money going out.")]
    [Guide("The running balance column helps you track the account balance over time, making it easy to verify that funds are properly accounted for.")]
    [Header("Working with Special Account Transactions")]
    [Guide("Transactions cannot be entered directly into special accounts. Instead, they are created automatically when you record transactions in other areas of the system and select this special account.")]
    [Guide("To view the source transaction, click on any transaction in the list. This will take you to the original entry where you can make changes if needed.")]
    [Columns]
    internal sealed class SpecialAccountTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid SpecialAccount;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForSpecialAccounts && x.SpecialAccount?.Key == SpecialAccount).OrderByDescending(x => x.Date).ToArray();
        }
    }
}
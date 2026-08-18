using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using HttpFramework;
using ManagerServer.Model;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.InterAccountTransfer))]
    [Guide("The *Inter Account Transfer* view displays detailed information about a transfer of funds between your bank and cash accounts.")]
    [Guide("This view shows the source account, destination account, amounts transferred, and exchange rates if the accounts use different currencies.")]
    [Guide("From this view, you can edit the transfer details by clicking the **Edit** button, or create a copy of the transfer using the **Copy to** menu.")]
    [LinkGuide("Learn more about creating transfers:", typeof(InterAccountTransferForm))]
    internal sealed class InterAccountTransferView : TransactionView<ManagerServer.Model.InterAccountTransfer>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(ManagerServer.Model.InterAccountTransfer), typeof(ManagerServer.Model.RecurringInterAccountTransfer)];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new InterAccountTransferTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
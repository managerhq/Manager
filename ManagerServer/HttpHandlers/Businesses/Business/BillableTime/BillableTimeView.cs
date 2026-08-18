using ManagerServer.Globalization;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.BillableTime
{
    [ProtoContract]
    [Title(nameof(Strings.BillableTime), nameof(Strings.View))]
    [Guide("The *Billable Time* view displays detailed information about a billable time entry that has been recorded for a customer.")]
    [Guide("This view shows key information including the date, description, time spent, hourly rate, and total amount to be billed to the customer.")]
    [Guide("The status of the billable time entry is prominently displayed, indicating whether it is *Uninvoiced*, *Invoiced*, or *Written Off*.")]
    [Guide("If the billable time has been invoiced, it will be linked to the corresponding sales invoice. Written-off entries show the date when they were written off.")]
    [LinkGuide("To edit this billable time entry, see:", typeof(BillableTimeEntryForm))]
    internal sealed class BillableTimeView : TransactionView<ManagerServer.Model.BillableTime>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [ typeof(ManagerServer.Model.BillableTime) ];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new BillableTimeTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringInterAccountTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringInterAccountTransfer))]
    [Guide("Set up recurring transfers between bank and cash accounts.")]
    [Guide("Useful for regular transfers like monthly cash floats or scheduled account sweeps.")]
    [Fields(typeof(ManagerServer.Model.RecurringInterAccountTransfer))]
    internal sealed class RecurringInterAccountTransferForm : NakedVueForm<ManagerServer.Model.RecurringInterAccountTransfer>
    {
        protected override void OnSource(RecurringInterAccountTransfer form, ManagerServer.Model.Object source)
        {
            if (source is InterAccountTransfer interAccountTransfer)
            {
                Copy(interAccountTransfer, form);
            }
        }
    }
}

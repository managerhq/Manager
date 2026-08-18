using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPayments
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringPayment), nameof(Strings.Edit))]
    [Guide("The Recurring Payment form is used to set up payments that repeat automatically.")]
    [Guide("Recurring payments are useful for regular expenses like rent, utilities, or subscriptions.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.RecurringPayment))]
    internal sealed class RecurringPaymentForm : NakedVueForm<ManagerServer.Model.RecurringPayment>
    {
        protected override void OnSource(RecurringPayment form, ManagerServer.Model.Object source)
        {
            if (source is Payment payment)
            {
                Copy(payment, form);
            }
        }
    }
}

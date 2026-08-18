using System;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    [Title(nameof(Strings.BankReconciliation), nameof(Strings.Edit))]
    [Guide("Use this form to reconcile your bank account balance in Manager with your actual bank statement.")]
    [Guide("Bank reconciliations ensure your records match the bank's records and help identify errors or missing transactions.")]
    [Header("Form Fields")]
    [Guide("Complete the following fields to create a bank reconciliation:")]
    [Fields(typeof(ManagerServer.Model.BankReconciliation))]
    internal sealed class BankReconciliationForm : NakedVueForm<ManagerServer.Model.BankReconciliation>
    {
        [ProtoMember(1)] public Guid? BankAccount;
        [ProtoMember(2)] public DateTime? Date;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(BankReconciliation form, ManagerServer.Model.Object source)
        {
            if (Date.HasValue) form.Date = Date.Value;
            if (BankAccount.HasValue) form.BankAccount = BankAccount;
        }
    }
}

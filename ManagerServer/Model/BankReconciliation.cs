using System;
using System.Collections.Generic;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("a3b1d610-b5e8-4f17-8e97-e53e69b78bb5")]
    public sealed class BankReconciliation : Object, IComparable<BankReconciliation>, ICustomFields
    {
        [Header("Overview")]
        [Guide("Enter the ending date shown on your bank statement that you are reconciling.")]
        [Guide("Bank reconciliations are essential for verifying that your accounting records match your actual bank balance.")]
        [Header("Why Reconcile")]
        [Guide("Regular reconciliations help identify errors, missing transactions, unauthorized charges, and timing differences between your records and the bank's records.")]
        [Guide("It is recommended to reconcile bank accounts at least monthly, or more frequently for high-volume accounts.")]
        [ProtoMember(3)] public DateTime Date { get; set; }
        [Guide("Select the bank or cash account you want to reconcile against your bank statement.")]
        [Guide("Each account must be reconciled separately using its corresponding bank statement.")]
        [Guide("Only accounts with cleared transactions will show meaningful reconciliation results.")]
        [ProtoMember(2), Autocomplete(typeof(BankOrCashAccount))] public Guid? BankAccount { get; set; }
        [Header("Statement Balance")]
        [Guide("Enter the closing balance exactly as shown on your bank statement for the reconciliation date.")]
        [Guide("The system will calculate the difference between this statement balance and your recorded cleared transactions.")]
        [Header("Common Reconciliation Differences")]
        [Guide("If your balance doesn't match, common causes include:")]
        [Guide("• Outstanding checks or deposits that haven't cleared the bank yet")]
        [Guide("• Bank fees or interest not yet recorded in your accounts")]
        [Guide("• Timing differences between transaction and clearing dates")]
        [Guide("• Data entry errors or missing transactions")]
        [Guide("Any unexplained differences should be investigated and resolved before completing the reconciliation.")]
        [ProtoMember(4), AppendCurrency(nameof(BankAccount))] public decimal StatementBalance { get; set; }
        [ProtoMember(5), TableColumn] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(6), TableColumn] public CustomFields CustomFields2 { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        int IComparable<BankReconciliation>.CompareTo(BankReconciliation other)
        {
            return (other.Date).CompareTo((Date));
        }
    }
}

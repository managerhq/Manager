using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    [Title(nameof(Strings.BankReconciliation))]
    [Guide("The *bank reconciliation* view displays a detailed comparison between your bank statement balance and your accounting records.")]
    [Guide("This view helps you identify and track transactions that haven't cleared your bank account yet.")]
    [Header("Key Information Displayed")]
    [Guide("The reconciliation shows your *closing balance as per bank* statement, which is the ending balance shown on your bank statement for the selected date.")]
    [Guide("*Pending deposits* are receipts or transfers that you've recorded but haven't appeared on your bank statement yet.")]
    [Guide("*Pending withdrawals* are payments or transfers that you've recorded but haven't cleared through your bank account.")]
    [Header("Reconciliation Status")]
    [Guide("The view calculates the *adjusted closing balance* by adding pending deposits and subtracting pending withdrawals from your bank statement balance.")]
    [Guide("This adjusted balance is compared to your *closing balance as per balance sheet* to determine if there's a discrepancy.")]
    [Guide("If the discrepancy is zero, your reconciliation shows as **Reconciled**. Otherwise, it displays **Not Reconciled** with the discrepancy amount.")]
    [LinkGuide("To create or edit reconciliations, see:", typeof(BankReconciliationForm))]
    internal sealed class BankReconciliationView : TransactionView<ManagerServer.Model.BankReconciliation>
    {
    }
}
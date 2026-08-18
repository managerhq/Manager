using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.InterAccountTransfer), nameof(Strings.Edit))]
    [Guide("The `Inter Account Transfer` form is used to transfer money between your bank accounts and cash accounts.")]
    [Guide("This form allows you to record movements of funds when you deposit cash into a bank account, withdraw cash from a bank account, or transfer money between different bank accounts.")]
    [Header("When to Use Inter Account Transfers")]
    [Guide("Use an inter account transfer when moving money between accounts that you control. Common examples include:")]
    [Guide("• Depositing cash takings into your bank account")]
    [Guide("• Withdrawing cash from the bank for petty cash")]
    [Guide("• Transferring funds between your business bank accounts")]
    [Guide("• Moving money from your main account to a savings account")]
    [Header("Important Notes")]
    [Guide("Inter account transfers do not affect your profit and loss statement, as they only move money between your own accounts.")]
    [Guide("If you need to record payments to suppliers or receipts from customers, use `Payment` or `Receipt` forms instead.")]
    [Header("Form Fields")]
    [Guide("Complete the following fields to record your transfer:")]
    [Fields(typeof(ManagerServer.Model.InterAccountTransfer))]
    internal sealed class InterAccountTransferForm : NakedVueForm<ManagerServer.Model.InterAccountTransfer>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(InterAccountTransfer form, ManagerServer.Model.Object source)
        {
            if (source is InterAccountTransfer interAccountTransfer)
            {
                Copy(interAccountTransfer, form);
            }
        }
    }
}
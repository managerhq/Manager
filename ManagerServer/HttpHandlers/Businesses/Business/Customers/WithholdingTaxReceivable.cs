using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Query;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Attributes;
using ManagerComponents;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.Customer), nameof(Strings.WithholdingTaxReceivable))]
    [Guide("This screen displays *withholding tax receivable* transactions for a specific customer.")]
    [Guide("When customers deduct withholding tax from their payments to you, these amounts appear here as receivables that can be claimed as tax credits from the government.")]
    [Guide("Each transaction shows the date, description, and amount of withholding tax that was deducted from your income.")]
    [Guide("You can use the **New Receipt** button to record when you receive credit for these withheld amounts or when they are applied against your tax obligations.")]
    [LinkGuide("Learn more about recording withholding tax receipts:", typeof(WithholdingTaxReceipts.WithholdingTaxReceiptForm))]
    internal sealed class WithholdingTaxReceivable : TransactionViewer
    {
        [ProtoMember(1)] public Guid Customer;

        protected override HeaderButton GetPrimaryButton()
        {
            return new HeaderButton()
            {
                Text = Strings.NewReceipt,
                Url = new WithholdingTaxReceipts.WithholdingTaxReceiptForm() { Business = Business, Source = Customer, Referrer = this.ToUrl() }.ToUrl()
            };
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.WithholdingTaxReceivable && x.Customer.Key == Customer);
        }
    }
}

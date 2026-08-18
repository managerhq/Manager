using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Query;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Attributes;
using ManagerComponents;

namespace ManagerServer.HttpHandlers.Businesses.Business.Suppliers
{
    [ProtoContract]
    [Title(nameof(Strings.WithholdingTaxPayable))]
    [Guide("The *Withholding Tax Payable* report displays all withholding tax amounts that have been deducted from payments to this supplier.")]
    [Guide("When you pay a supplier and withhold tax from the payment, the withheld amount appears here as a liability that you owe to the tax authorities.")]
    [Guide("This report helps you track how much withholding tax you need to remit to the government for payments made to this specific supplier.")]
    [Guide("To record a payment to the tax authorities for the withholding tax liability, click the **New Payment** button at the top of the report.")]
    [Columns]
    internal sealed class WithholdingTaxPayable : TransactionViewer
    {
        [ProtoMember(1)] public Guid Supplier;

        protected override HeaderButton GetPrimaryButton()
        {
            return new HeaderButton()
            {
                Text = Strings.NewPayment,
                Url = new Payments.PaymentForm() { Business = Business, Source = Supplier, WithholdingTaxPayable = true, Referrer = this.ToUrl() }.ToUrl()
            };
        }

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var withholdingTaxPayableKey = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BalanceSheetWithholdingTaxPayableAccount));
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.Key == withholdingTaxPayableKey && x.Supplier.Key == Supplier);
        }
    }
}

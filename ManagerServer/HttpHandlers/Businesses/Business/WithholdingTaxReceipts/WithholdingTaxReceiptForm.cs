using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    [Title(nameof(Strings.WithholdingTaxReceipt), nameof(Strings.Edit))]
    [Guide("The Withholding Tax Receipt form is used to record tax withheld on behalf of customers.")]
    [Guide("This documents tax amounts that have been withheld and will be remitted to tax authorities.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.WithholdingTaxReceipt))]
    internal sealed class WithholdingTaxReceiptForm : NakedVueForm<ManagerServer.Model.WithholdingTaxReceipt>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(WithholdingTaxReceipt form, ManagerServer.Model.Object source)
        {
            if (source is Customer customer)
            {
                form.Customer = customer.Key;
            }
        }
    }
}
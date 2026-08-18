using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using HttpFramework;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.SalesInvoice), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for sales invoice.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(SalesInvoiceStartingBalanceList))]
    internal sealed class SalesInvoiceStartingBalanceForm : NakedVueForm<SalesInvoiceStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}
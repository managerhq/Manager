using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomerPortals
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerPortal))]
    [Guide("Configure a customer portal for self-service access.")]
    [Guide("Customers can view invoices, statements, and make payments online.")]
    [Fields(typeof(ManagerServer.Model.CustomerPortal))]
    internal sealed class CustomerPortalForm : NakedVueForm<ManagerServer.Model.CustomerPortal>
    {
    }
}

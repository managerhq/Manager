using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DeliveryNotes))]
    [Guide("This utility helps maintain data integrity by removing duplicate *delivery notes* linked to the same *sales invoice*.")]
    [Guide("In some cases, multiple *delivery notes* may inadvertently become associated with a single *sales invoice*, which can cause confusion and reporting issues.")]
    [Guide("This tool automatically identifies and removes the duplicate entries, ensuring each *sales invoice* has only one associated *delivery note*.")]
    [Guide("The utility keeps the first *delivery note* for each *sales invoice* and removes any subsequent duplicates.")]
    internal sealed class FixDeliveryNotes : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var deliveryNotes = ApplicationData.Businesses.Get(Business)
                .OfType<DeliveryNote>()
                .Where(x => x.SalesInvoice.HasValue)
                .GroupBy(x => x.SalesInvoice.Value)
                .SelectMany(x => x.Skip(1))
                .ToArray();

            ApplicationData.Businesses.Process(Business, deliveryNotes.Select(x => x.Key).ToArray(), GetUserName());

            Write("OK - "+deliveryNotes.Length.ToString());
        }
    }
}

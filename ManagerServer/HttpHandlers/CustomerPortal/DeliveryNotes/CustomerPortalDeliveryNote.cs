using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.CustomerPortal.DeliveryNotes
{
    [ProtoContract]
    class CustomerPortalDeliveryNote : View<ManagerServer.Api.Businesses.Business.DeliveryNotes.GetDeliveryNoteView>
    {
    }
}
